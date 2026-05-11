// ============================================================
//  FILE 14 — InventoryService
//  Method tested: IssueItemAsync()
// ============================================================

using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.DTOs.Inventory;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IGenericRepository<ItemIssue>> _issueRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _issueRepoMock = new Mock<IGenericRepository<ItemIssue>>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new InventoryService(
            _inventoryRepoMock.Object,
            _issueRepoMock.Object,
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task IssueItemAsync_WhenItemNotFound_ReturnsFailure()
    {
        // Arrange
        var dto = new IssueItemDto { InventoryItemId = 99, QuantityIssued = 5 };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        var (success, message) = await _service.IssueItemAsync(dto, "user-wm-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Inventory item not found.", message);
    }

    [Fact]
    public async Task IssueItemAsync_WhenInsufficientStock_ReturnsFailure()
    {
        // Arrange
        var dto = new IssueItemDto { InventoryItemId = 1, QuantityIssued = 100 };
        var item = new InventoryItem { Id = 1, ItemName = "Laptop", QuantityInStock = 10 };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(item);

        // Act
        var (success, message) = await _service.IssueItemAsync(dto, "user-wm-1");

        // Assert
        Assert.False(success);
        Assert.Contains("Insufficient stock", message);
    }

    [Fact]
    public async Task IssueItemAsync_WhenSufficientStock_DeductsAndSaves()
    {
        // Arrange
        var dto = new IssueItemDto
        {
            InventoryItemId = 1,
            QuantityIssued = 3,
            IssuedTo = "HR Dept",
            IssueDate = DateTime.UtcNow
        };

        var item = new InventoryItem
        {
            Id = 1,
            ItemName = "Laptop",
            QuantityInStock = 10,
            Unit = "Units"
        };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _inventoryRepoMock.Setup(r => r.Update(It.IsAny<InventoryItem>()));
        _issueRepoMock.Setup(r => r.AddAsync(It.IsAny<ItemIssue>()))
            .Returns(Task.CompletedTask);
        _inventoryRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var (success, message) = await _service.IssueItemAsync(dto, "user-wm-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Item issued successfully.", message);
        Assert.Equal(7, item.QuantityInStock); // 10 - 3
    }

    [Fact]
    public async Task IssueItemAsync_WhenLowStockAfterIssue_CreatesLowStockNotification()
    {
        // Arrange
        var dto = new IssueItemDto
        {
            InventoryItemId = 1,
            QuantityIssued = 5,
            IssuedTo = "IT Dept",
            IssueDate = DateTime.UtcNow
        };

        // Stock will drop to 5 after issuing (≤ 10 triggers alert)
        var item = new InventoryItem
        {
            Id = 1,
            ItemName = "Keyboard",
            QuantityInStock = 10,
            Unit = "Units"
        };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _inventoryRepoMock.Setup(r => r.Update(It.IsAny<InventoryItem>()));
        _issueRepoMock.Setup(r => r.AddAsync(It.IsAny<ItemIssue>()))
            .Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _inventoryRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var (success, _) = await _service.IssueItemAsync(dto, "user-wm-1");

        // Assert
        Assert.True(success);
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.Is<Notification>(n => n.Type == "LowStockAlert")),
            Times.Once);
    }
}