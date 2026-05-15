using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.Inventory;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class InventoryServiceTests
{
    private Mock<IInventoryRepository> _inventoryRepoMock;
    private Mock<IGenericRepository<ItemIssue>> _issueRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<IMapper> _mapperMock;
    private InventoryService _service;

    [SetUp]
    public void SetUp()
    {
        _inventoryRepoMock    = new Mock<IInventoryRepository>();
        _issueRepoMock        = new Mock<IGenericRepository<ItemIssue>>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        _service = new InventoryService(_inventoryRepoMock.Object, _issueRepoMock.Object, _notificationRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task IssueItemAsync_WhenItemNotFound_ReturnsFailure()
    {
        _inventoryRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((InventoryItem?)null);

        var (success, message) = await _service.IssueItemAsync(new IssueItemDto { InventoryItemId = 99, QuantityIssued = 5 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Inventory item not found."));
    }

    [Test]
    public async Task IssueItemAsync_WhenInsufficientStock_ReturnsFailure()
    {
        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new InventoryItem { Id = 1, QuantityInStock = 10 });

        var (success, message) = await _service.IssueItemAsync(new IssueItemDto { InventoryItemId = 1, QuantityIssued = 100 }, "u1");

        Assert.That(success,            Is.False);
        Assert.That(message, Does.Contain("Insufficient stock"));
    }

    [Test]
    public async Task IssueItemAsync_WhenSufficientStock_DeductsAndSaves()
    {
        var item = new InventoryItem { Id = 1, ItemName = "Laptop", QuantityInStock = 10, Unit = "Units" };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _inventoryRepoMock.Setup(r => r.Update(It.IsAny<InventoryItem>()));
        _issueRepoMock.Setup(r => r.AddAsync(It.IsAny<ItemIssue>())).Returns(Task.CompletedTask);
        _inventoryRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var dto = new IssueItemDto { InventoryItemId = 1, QuantityIssued = 3, IssuedTo = "HR", IssueDate = DateTime.UtcNow };
        var (success, message) = await _service.IssueItemAsync(dto, "u1");

        Assert.That(success,             Is.True);
        Assert.That(message,             Is.EqualTo("Item issued successfully."));
        Assert.That(item.QuantityInStock, Is.EqualTo(7));
    }

    [Test]
    public async Task IssueItemAsync_WhenLowStockAfterIssue_CreatesLowStockNotification()
    {
        var item = new InventoryItem { Id = 1, ItemName = "Keyboard", QuantityInStock = 10, Unit = "Units" };

        _inventoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        _inventoryRepoMock.Setup(r => r.Update(It.IsAny<InventoryItem>()));
        _issueRepoMock.Setup(r => r.AddAsync(It.IsAny<ItemIssue>())).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _inventoryRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var dto = new IssueItemDto { InventoryItemId = 1, QuantityIssued = 5, IssuedTo = "IT", IssueDate = DateTime.UtcNow };
        await _service.IssueItemAsync(dto, "u1");

        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n => n.Type == "LowStockAlert")), Times.Once);
    }
}
