using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.PurchaseOrder;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class PurchaseOrderServiceTests
{
    private Mock<IPurchaseOrderRepository> _poRepoMock;
    private Mock<IContractRepository> _contractRepoMock;
    private Mock<IVendorRepository> _vendorRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<IMapper> _mapperMock;
    private PurchaseOrderService _service;

    [SetUp]
    public void SetUp()
    {
        _poRepoMock           = new Mock<IPurchaseOrderRepository>();
        _contractRepoMock     = new Mock<IContractRepository>();
        _vendorRepoMock       = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        _service = new PurchaseOrderService(_poRepoMock.Object, _contractRepoMock.Object, _vendorRepoMock.Object, _notificationRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreatePOAsync_WhenContractNotFound_ReturnsFailure()
    {
        _contractRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Contract?)null);

        var (success, message, data) = await _service.CreatePOAsync(new CreatePurchaseOrderDto { ContractId = 99 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Contract not found."));
        Assert.That(data,    Is.Null);
    }

    [Test]
    public async Task CreatePOAsync_WhenContractNotActive_ReturnsFailure()
    {
        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Contract { Id = 1, Status = ContractStatus.Draft });

        var (success, message, _) = await _service.CreatePOAsync(new CreatePurchaseOrderDto { ContractId = 1 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("An active contract is required to create a PO."));
    }

    [Test]
    public async Task CreatePOAsync_WhenVendorNotFound_ReturnsFailure()
    {
        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Contract { Id = 1, Status = ContractStatus.Active });
        _vendorRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vendor?)null);

        var (success, message, _) = await _service.CreatePOAsync(new CreatePurchaseOrderDto { ContractId = 1, VendorId = 99 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Vendor not found."));
    }

    [Test]
    public async Task CreatePOAsync_WhenValid_CreatesPOAndNotifiesVendor()
    {
        var contract  = new Contract { Id = 1, Status = ContractStatus.Active };
        var vendor    = new Vendor   { Id = 1, UserId = "vendor-u", CompanyName = "Acme" };
        var savedPO   = new PurchaseOrder { Id = 10, PONumber = "PO-001" };
        var poDto     = new PurchaseOrderDto { Id = 10, PONumber = "PO-001" };

        _contractRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);
        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _poRepoMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>())).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _poRepoMock.Setup(r => r.Update(It.IsAny<PurchaseOrder>()));
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(savedPO);
        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(savedPO)).Returns(poDto);

        var dto = new CreatePurchaseOrderDto
        {
            ContractId = 1, VendorId = 1,
            Items = new List<CreatePOItemDto> { new CreatePOItemDto { ItemName = "Desk", Quantity = 2, UnitPrice = 150m } }
        };

        var (success, message, data) = await _service.CreatePOAsync(dto, "u1");

        Assert.That(success, Is.True);
        Assert.That(message, Is.EqualTo("Purchase Order created and sent to vendor."));
        Assert.That(data,    Is.Not.Null);
        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n => n.Type == "POCreated")), Times.Once);
    }
}
