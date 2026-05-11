// ============================================================
//  FILE 17 — PurchaseOrderService
//  Method tested: CreatePOAsync()
// ============================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.DTOs.PurchaseOrder;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class PurchaseOrderServiceTests
{
    private readonly Mock<IPurchaseOrderRepository> _poRepoMock;
    private readonly Mock<IContractRepository> _contractRepoMock;
    private readonly Mock<IVendorRepository> _vendorRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PurchaseOrderService _service;

    public PurchaseOrderServiceTests()
    {
        _poRepoMock = new Mock<IPurchaseOrderRepository>();
        _contractRepoMock = new Mock<IContractRepository>();
        _vendorRepoMock = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new PurchaseOrderService(
            _poRepoMock.Object,
            _contractRepoMock.Object,
            _vendorRepoMock.Object,
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreatePOAsync_WhenContractNotFound_ReturnsFailure()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto { ContractId = 99 };

        _contractRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Contract?)null);

        // Act
        var (success, message, data) = await _service.CreatePOAsync(dto, "user-po-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Contract not found.", message);
        Assert.Null(data);
    }

    [Fact]
    public async Task CreatePOAsync_WhenContractNotActive_ReturnsFailure()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto { ContractId = 1 };
        var contract = new Contract { Id = 1, Status = ContractStatus.Draft };

        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

        // Act
        var (success, message, _) = await _service.CreatePOAsync(dto, "user-po-1");

        // Assert
        Assert.False(success);
        Assert.Equal("An active contract is required to create a PO.", message);
    }

    [Fact]
    public async Task CreatePOAsync_WhenVendorNotFound_ReturnsFailure()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto { ContractId = 1, VendorId = 99 };
        var contract = new Contract { Id = 1, Status = ContractStatus.Active };

        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);
        _vendorRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Vendor?)null);

        // Act
        var (success, message, _) = await _service.CreatePOAsync(dto, "user-po-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Vendor not found.", message);
    }

    [Fact]
    public async Task CreatePOAsync_WhenValid_CreatesPOAndNotifiesVendor()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto
        {
            ContractId = 1,
            VendorId = 1,
            Items = new List<CreatePOItemDto>
            {
                new CreatePOItemDto { ItemName = "Desk", Quantity = 2, UnitPrice = 150m }
            }
        };

        var contract = new Contract { Id = 1, Status = ContractStatus.Active };
        var vendor = new Vendor { Id = 1, UserId = "vendor-user", CompanyName = "Acme" };
        var savedPO = new PurchaseOrder { Id = 10, PONumber = "PO-00000001" };
        var poDto = new PurchaseOrderDto { Id = 10, PONumber = "PO-00000001" };

        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);
        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _poRepoMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>()))
            .Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.Update(It.IsAny<PurchaseOrder>()));
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(savedPO);
        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(savedPO)).Returns(poDto);

        // Act
        var (success, message, data) = await _service.CreatePOAsync(dto, "user-po-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Purchase Order created and sent to vendor.", message);
        Assert.NotNull(data);
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.Is<Notification>(n => n.Type == "POCreated")), Times.Once);
    }
}