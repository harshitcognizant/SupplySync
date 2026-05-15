using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.GoodsReceipt;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class GoodsReceiptServiceTests
{
    private Mock<IGoodsReceiptRepository> _grRepoMock;
    private Mock<IPurchaseOrderRepository> _poRepoMock;
    private Mock<IInventoryRepository> _inventoryRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<IMapper> _mapperMock;
    private GoodsReceiptService _service;

    [SetUp]
    public void SetUp()
    {
        _grRepoMock           = new Mock<IGoodsReceiptRepository>();
        _poRepoMock           = new Mock<IPurchaseOrderRepository>();
        _inventoryRepoMock    = new Mock<IInventoryRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        _service = new GoodsReceiptService(_grRepoMock.Object, _poRepoMock.Object, _inventoryRepoMock.Object, _notificationRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateGoodsReceiptAsync_WhenPONotFound_ReturnsFailure()
    {
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(99)).ReturnsAsync((PurchaseOrder?)null);

        var (success, message, data) = await _service.CreateGoodsReceiptAsync(new CreateGoodsReceiptDto { PurchaseOrderId = 99 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Purchase Order not found."));
        Assert.That(data,    Is.Null);
    }

    [Test]
    public async Task CreateGoodsReceiptAsync_WhenPONotDelivered_ReturnsFailure()
    {
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1)).ReturnsAsync(new PurchaseOrder { Id = 1, Status = POStatus.Sent });

        var (success, message, _) = await _service.CreateGoodsReceiptAsync(new CreateGoodsReceiptDto { PurchaseOrderId = 1 }, "u1");

        Assert.That(success,            Is.False);
        Assert.That(message, Does.Contain("Vendor has not marked"));
    }

    [Test]
    public async Task CreateGoodsReceiptAsync_WhenNonRejectedGRExists_ReturnsDuplicateFailure()
    {
        // Only non-rejected GRs block re-creation
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1)).ReturnsAsync(new PurchaseOrder { Id = 1, Status = POStatus.Delivered });

        _grRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<GoodsReceipt, bool>>>()))
            .ReturnsAsync(new List<GoodsReceipt>
            {
                new GoodsReceipt { Id = 5, PurchaseOrderId = 1, Status = GoodsReceiptStatus.Accepted }
            });

        var (success, message, _) = await _service.CreateGoodsReceiptAsync(new CreateGoodsReceiptDto { PurchaseOrderId = 1 }, "u1");

        Assert.That(success,            Is.False);
        Assert.That(message, Does.Contain("already exists"));
    }

    [Test]
    public async Task CreateGoodsReceiptAsync_WhenOnlyRejectedGRExists_AllowsNewGR()
    {
        // Rejected GR should NOT block new GR creation (vendor re-delivering)
        var vendor = new Vendor { Id = 1, UserId = "vendor-u1" };
        var po     = new PurchaseOrder { Id = 1, Status = POStatus.Delivered, Vendor = vendor, CreatedByUserId = "u-po", PONumber = "PO-001" };

        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1)).ReturnsAsync(po);

        // Only rejected GR exists — should not trigger duplicate block
        _grRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<GoodsReceipt, bool>>>()))
            .ReturnsAsync(new List<GoodsReceipt>
            {
                new GoodsReceipt { Id = 3, PurchaseOrderId = 1, Status = GoodsReceiptStatus.Rejected }
            });

        _grRepoMock.Setup(r => r.AddAsync(It.IsAny<GoodsReceipt>())).Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.Update(It.IsAny<PurchaseOrder>()));
        _grRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _inventoryRepoMock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((InventoryItem?)null);
        _inventoryRepoMock.Setup(r => r.AddAsync(It.IsAny<InventoryItem>())).Returns(Task.CompletedTask);

        var grDto = new GoodsReceiptDto { Id = 10, GRNumber = "GR-002", Status = "Accepted" };
        _grRepoMock.Setup(r => r.GetGRWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new GoodsReceipt());
        _mapperMock.Setup(m => m.Map<GoodsReceiptDto>(It.IsAny<GoodsReceipt>())).Returns(grDto);

        var dto = new CreateGoodsReceiptDto
        {
            PurchaseOrderId = 1,
            Status          = "Accepted",
            Remarks         = "Re-delivery accepted",
            Items           = new List<GoodsReceiptItemDto> { new GoodsReceiptItemDto { ItemName = "Laptop", ReceivedQuantity = 5, Condition = "Good" } }
        };

        var (success, message, data) = await _service.CreateGoodsReceiptAsync(dto, "u-wm");

        Assert.That(success, Is.True);
        Assert.That(data,    Is.Not.Null);
    }

    [Test]
    public async Task CreateGoodsReceiptAsync_WhenRejected_RevertsPOToSent()
    {
        var vendor = new Vendor { Id = 1, UserId = "vendor-u1" };
        var po     = new PurchaseOrder { Id = 1, Status = POStatus.Delivered, Vendor = vendor, CreatedByUserId = "u-po", PONumber = "PO-001" };

        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1)).ReturnsAsync(po);
        _grRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<System.Func<GoodsReceipt, bool>>>())).ReturnsAsync(new List<GoodsReceipt>());
        _grRepoMock.Setup(r => r.AddAsync(It.IsAny<GoodsReceipt>())).Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.Update(It.IsAny<PurchaseOrder>()));
        _grRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);

        var grDto = new GoodsReceiptDto { Id = 10, Status = "Rejected" };
        _grRepoMock.Setup(r => r.GetGRWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new GoodsReceipt());
        _mapperMock.Setup(m => m.Map<GoodsReceiptDto>(It.IsAny<GoodsReceipt>())).Returns(grDto);

        var dto = new CreateGoodsReceiptDto
        {
            PurchaseOrderId = 1,
            Status          = "Rejected",
            Remarks         = "Damaged goods",
            Items           = new List<GoodsReceiptItemDto> { new GoodsReceiptItemDto { ItemName = "Laptop", ReceivedQuantity = 5, Condition = "Good" } }
        };

        await _service.CreateGoodsReceiptAsync(dto, "u-wm");

        // PO status should be reverted to Sent so vendor can re-deliver
        Assert.That(po.Status, Is.EqualTo(POStatus.Sent));
    }
}
