// ============================================================
//  FILE 12 — ContractService
//  Method tested: CreateContractAsync()
// ============================================================

using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.DTOs.Contract;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contractRepoMock;
    private readonly Mock<IVendorRepository> _vendorRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        _contractRepoMock = new Mock<IContractRepository>();
        _vendorRepoMock = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new ContractService(
            _contractRepoMock.Object,
            _vendorRepoMock.Object,
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CreateContractAsync_WhenVendorNotFound_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateContractDto { VendorId = 99 };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Vendor?)null);

        // Act
        var (success, message, data) =
            await _service.CreateContractAsync(dto, "user-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Vendor not found.", message);
        Assert.Null(data);
    }

    [Fact]
    public async Task CreateContractAsync_WhenVendorNotApproved_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateContractDto { VendorId = 1 };
        var vendor = new Vendor { Id = 1, Status = VendorStatus.Pending };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(vendor);

        // Act
        var (success, message, _) =
            await _service.CreateContractAsync(dto, "user-1");

        // Assert
        Assert.False(success);
        Assert.Equal(
            "Vendor must be approved before creating a contract.", message);
    }

    [Fact]
    public async Task CreateContractAsync_WhenVendorApproved_CreatesAndNotifies()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            VendorId = 1,
            PaymentTerms = "Net 30",
            DeliveryTerms = "FOB"
        };

        var vendor = new Vendor
        {
            Id = 1,
            UserId = "vendor-user-1",
            Status = VendorStatus.Approved,
            CompanyName = "Acme Corp"
        };

        var savedContract = new Contract
        {
            Id = 10,
            ContractNumber = "CON-12345678",
            VendorId = 1,
            Vendor = vendor
        };

        var contractDto = new ContractDto
        {
            Id = 10,
            VendorName = "Acme Corp",
            Status = "Draft"
        };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(vendor);

        _contractRepoMock.Setup(r => r.AddAsync(It.IsAny<Contract>()))
            .Returns(Task.CompletedTask);

        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);

        _contractRepoMock.Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        _contractRepoMock
            .Setup(r => r.GetContractWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(savedContract);

        _mapperMock.Setup(m => m.Map<ContractDto>(savedContract))
            .Returns(contractDto);

        // Act
        var (success, message, data) =
            await _service.CreateContractAsync(dto, "user-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Contract created successfully.", message);
        Assert.NotNull(data);

        _notificationRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Notification>()), Times.Once);
    }
}