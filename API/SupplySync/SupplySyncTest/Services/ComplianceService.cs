// ============================================================
//  FILE 11 — ComplianceService
//  Method tested: PerformCheckAsync()
// ============================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using SupplySync.API.DTOs.Compliance;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class ComplianceServiceTests
{
    private readonly Mock<IComplianceRepository> _complianceRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ComplianceService _service;

    public ComplianceServiceTests()
    {
        _complianceRepoMock = new Mock<IComplianceRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _mapperMock = new Mock<IMapper>();

        _service = new ComplianceService(
            _complianceRepoMock.Object,
            _notificationRepoMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task PerformCheckAsync_WithInvalidStatus_ReturnsFailure()
    {
        // Arrange
        var dto = new CreateComplianceCheckDto
        {
            EntityType = "Vendor",
            EntityId = 1,
            Status = "InvalidStatus"
        };

        // Act
        var (success, message, data) =
            await _service.PerformCheckAsync(dto, "user-co-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Invalid status. Use Pass or Fail.", message);
        Assert.Null(data);
    }

    [Fact]
    public async Task PerformCheckAsync_WithPassStatus_SavesAndReturnsSuccess()
    {
        // Arrange
        var dto = new CreateComplianceCheckDto
        {
            EntityType = "Contract",
            EntityId = 3,
            Status = "Pass",
            Remarks = "All clear"
        };

        var expectedDto = new ComplianceCheckDto
        {
            EntityType = "Contract",
            EntityId = 3,
            Status = "Pass"
        };

        _complianceRepoMock
            .Setup(r => r.AddAsync(It.IsAny<ComplianceCheck>()))
            .Returns(Task.CompletedTask);

        _complianceRepoMock
            .Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<ComplianceCheckDto>(It.IsAny<ComplianceCheck>()))
            .Returns(expectedDto);

        // Act
        var (success, message, data) =
            await _service.PerformCheckAsync(dto, "user-co-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Compliance check recorded.", message);
        Assert.NotNull(data);
        Assert.Equal("Pass", data!.Status);
    }

    [Fact]
    public async Task PerformCheckAsync_WithFailStatus_NotifiesAdminAndProcurement()
    {
        // Arrange
        var dto = new CreateComplianceCheckDto
        {
            EntityType = "Vendor",
            EntityId = 5,
            Status = "Fail",
            Remarks = "Missing license"
        };

        var adminUser = new ApplicationUser { Id = "admin-1" };
        var procUser = new ApplicationUser { Id = "proc-1" };

        _userManagerMock
            .Setup(u => u.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(new List<ApplicationUser> { adminUser });

        _userManagerMock
            .Setup(u => u.GetUsersInRoleAsync("ProcurementOfficer"))
            .ReturnsAsync(new List<ApplicationUser> { procUser });

        _complianceRepoMock.Setup(r => r.AddAsync(It.IsAny<ComplianceCheck>()))
            .Returns(Task.CompletedTask);
        _complianceRepoMock.Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ComplianceCheckDto>(It.IsAny<ComplianceCheck>()))
            .Returns(new ComplianceCheckDto { Status = "Fail" });

        // Act
        var (success, _, _) = await _service.PerformCheckAsync(dto, "user-co-1");

        // Assert
        Assert.True(success);
        // 2 notifications — one for Admin, one for ProcurementOfficer
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Notification>()), Times.Exactly(2));
    }
}