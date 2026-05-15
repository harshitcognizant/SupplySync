using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.Contract;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class ContractServiceTests
{
    private Mock<IContractRepository> _contractRepoMock;
    private Mock<IVendorRepository> _vendorRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<IMapper> _mapperMock;
    private ContractService _service;

    [SetUp]
    public void SetUp()
    {
        _contractRepoMock     = new Mock<IContractRepository>();
        _vendorRepoMock       = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        _service = new ContractService(_contractRepoMock.Object, _vendorRepoMock.Object, _notificationRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task CreateContractAsync_WhenVendorNotFound_ReturnsFailure()
    {
        _vendorRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vendor?)null);

        var (success, message, data) = await _service.CreateContractAsync(new CreateContractDto { VendorId = 99 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Vendor not found."));
        Assert.That(data,    Is.Null);
    }

    [Test]
    public async Task CreateContractAsync_WhenVendorNotApproved_ReturnsFailure()
    {
        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Vendor { Id = 1, Status = VendorStatus.Pending });

        var (success, message, _) = await _service.CreateContractAsync(new CreateContractDto { VendorId = 1 }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Vendor must be approved before creating a contract."));
    }

    [Test]
    public async Task CreateContractAsync_WhenVendorApproved_CreatesAndNotifies()
    {
        var vendor      = new Vendor { Id = 1, UserId = "vendor-u1", Status = VendorStatus.Approved, CompanyName = "Acme" };
        var savedContract = new Contract { Id = 10, ContractNumber = "CON-001", VendorId = 1, Vendor = vendor };
        var contractDto = new ContractDto { Id = 10, Status = "Draft" };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _contractRepoMock.Setup(r => r.AddAsync(It.IsAny<Contract>())).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _contractRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _contractRepoMock.Setup(r => r.GetContractWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(savedContract);
        _mapperMock.Setup(m => m.Map<ContractDto>(savedContract)).Returns(contractDto);

        var (success, message, data) = await _service.CreateContractAsync(new CreateContractDto { VendorId = 1 }, "u1");

        Assert.That(success, Is.True);
        Assert.That(message, Is.EqualTo("Contract created successfully."));
        Assert.That(data,    Is.Not.Null);
        _notificationRepoMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Once);
    }
}
