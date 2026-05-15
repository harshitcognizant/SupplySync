using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.Compliance;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class ComplianceServiceTests
{
    private Mock<IComplianceRepository> _complianceRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IMapper> _mapperMock;
    private ComplianceService _service;

    [SetUp]
    public void SetUp()
    {
        _complianceRepoMock    = new Mock<IComplianceRepository>();
        _notificationRepoMock  = new Mock<IGenericRepository<Notification>>();
        _mapperMock            = new Mock<IMapper>();

        var store          = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock   = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        _service = new ComplianceService(_complianceRepoMock.Object, _notificationRepoMock.Object, _userManagerMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task PerformCheckAsync_WithInvalidStatus_ReturnsFailure()
    {
        var dto = new CreateComplianceCheckDto { EntityType = "Vendor", EntityId = 1, Status = "InvalidStatus" };

        var (success, message, data) = await _service.PerformCheckAsync(dto, "user-co-1");

        Assert.That(success,  Is.False);
        Assert.That(message,  Is.EqualTo("Invalid status. Use Pass or Fail."));
        Assert.That(data,     Is.Null);
    }

    [Test]
    public async Task PerformCheckAsync_WithPassStatus_SavesAndReturnsSuccess()
    {
        var dto = new CreateComplianceCheckDto { EntityType = "Contract", EntityId = 3, Status = "Pass", Remarks = "All clear" };
        var expectedDto = new ComplianceCheckDto { Status = "Pass" };

        _complianceRepoMock.Setup(r => r.AddAsync(It.IsAny<ComplianceCheck>())).Returns(Task.CompletedTask);
        _complianceRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ComplianceCheckDto>(It.IsAny<ComplianceCheck>())).Returns(expectedDto);

        var (success, message, data) = await _service.PerformCheckAsync(dto, "user-co-1");

        Assert.That(success, Is.True);
        Assert.That(message, Is.EqualTo("Compliance check recorded."));
        Assert.That(data!.Status, Is.EqualTo("Pass"));
    }

    [Test]
    public async Task PerformCheckAsync_WithFailStatus_NotifiesAdminAndProcurement()
    {
        var dto = new CreateComplianceCheckDto { EntityType = "Vendor", EntityId = 5, Status = "Fail", Remarks = "Missing license" };

        _userManagerMock.Setup(u => u.GetUsersInRoleAsync("Admin")).ReturnsAsync(new List<ApplicationUser> { new ApplicationUser { Id = "admin-1" } });
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync("ProcurementOfficer")).ReturnsAsync(new List<ApplicationUser> { new ApplicationUser { Id = "proc-1" } });
        _complianceRepoMock.Setup(r => r.AddAsync(It.IsAny<ComplianceCheck>())).Returns(Task.CompletedTask);
        _complianceRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ComplianceCheckDto>(It.IsAny<ComplianceCheck>())).Returns(new ComplianceCheckDto { Status = "Fail" });

        var (success, _, _) = await _service.PerformCheckAsync(dto, "user-co-1");

        Assert.That(success, Is.True);
        _notificationRepoMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Exactly(2));
    }
}
