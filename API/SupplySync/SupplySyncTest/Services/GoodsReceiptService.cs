// ============================================================
//  FILE 13 — GoodsReceiptService
//  Method tested: CreateGoodsReceiptAsync()
// ============================================================

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.DTOs.GoodsReceipt;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class GoodsReceiptServiceTests
{
    private readonly Mock<IGoodsReceiptRepository> _grRepoMock;
    private readonly Mock<IPurchaseOrderRepository> _poRepoMock;
    private readonly Mock<IInventoryRepository> _inventoryRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GoodsReceiptService _service;

    public GoodsReceiptServiceTests()
    {
        _grRepoMock = new Mock<IGoodsReceiptRepository>();
        _poRepoMock = new Mock<IPurchaseOrderRepository>();
        _inventoryRepoMock = new Mock<IInventoryRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new GoodsReceiptService(
            _grRepoMock.Object,
            _poRepoMock.Object,
            _inventoryRepoMock.Object,
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateGoodsReceiptAsync_WhenPONotFound_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 99 };

        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(99))
            .ReturnsAsync((PurchaseOrder?)null);

        // Act
        var (success, message, data) =
            await _service.CreateGoodsReceiptAsync(dto, "user-wm-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Purchase Order not found.", message);
        Assert.Null(data);
    }

    [Fact]
    public async Task CreateGoodsReceiptAsync_WhenPONotDelivered_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 1 };
        var po = new PurchaseOrder { Id = 1, Status = POStatus.Sent };

        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1))
            .ReturnsAsync(po);

        // Act
        var (success, message, _) =
            await _service.CreateGoodsReceiptAsync(dto, "user-wm-1");

        // Assert
        Assert.False(success);
        Assert.Contains("Vendor has not marked", message);
    }

    [Fact]
    public async Task CreateGoodsReceiptAsync_WhenDuplicateGR_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 1 };
        var po = new PurchaseOrder { Id = 1, Status = POStatus.Delivered };

        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1))
            .ReturnsAsync(po);

        // Return an existing GR so duplicate check fires
        _grRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<GoodsReceipt, bool>>>()))
            .ReturnsAsync(new List<GoodsReceipt>
            {
                new GoodsReceipt { Id = 5, PurchaseOrderId = 1 }
            });

        // Act
        var (success, message, _) =
            await _service.CreateGoodsReceiptAsync(dto, "user-wm-1");

        // Assert
        Assert.False(success);
        Assert.Contains("already exists", message);
    }
}