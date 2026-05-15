using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.Vendor;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class VendorServiceTests
{
    private Mock<IVendorRepository> _vendorRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IMapper> _mapperMock;
    private VendorService _service;

    [SetUp]
    public void SetUp()
    {
        _vendorRepoMock       = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        var store        = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        _service = new VendorService(_vendorRepoMock.Object, _notificationRepoMock.Object, _userManagerMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task UpdateVendorStatusAsync_WhenVendorNotFound_ReturnsFailure()
    {
        _vendorRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Vendor?)null);

        var (success, message) = await _service.UpdateVendorStatusAsync(99, new VendorStatusUpdateDto { Status = "Approved" }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Vendor not found."));
    }

    [Test]
    public async Task UpdateVendorStatusAsync_WhenInvalidStatus_ReturnsFailure()
    {
        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Vendor { Id = 1, UserId = "v-u" });

        var (success, message) = await _service.UpdateVendorStatusAsync(1, new VendorStatusUpdateDto { Status = "InvalidStatus" }, "u1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Invalid status value."));
    }

    [Test]
    public async Task UpdateVendorStatusAsync_WhenApproved_UpdatesAndNotifies()
    {
        var vendor = new Vendor { Id = 1, UserId = "v-u", CompanyName = "Acme", Status = VendorStatus.Pending };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _vendorRepoMock.Setup(r => r.Update(It.IsAny<Vendor>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _vendorRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var (success, message) = await _service.UpdateVendorStatusAsync(1, new VendorStatusUpdateDto { Status = "Approved" }, "u1");

        Assert.That(success,       Is.True);
        Assert.That(message,       Is.EqualTo("Vendor status updated to Approved."));
        Assert.That(vendor.Status, Is.EqualTo(VendorStatus.Approved));
        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n => n.Type == "VendorStatusUpdate")), Times.Once);
    }

    [Test]
    public async Task UpdateVendorStatusAsync_WhenRejected_SetsRejectionReason()
    {
        var vendor = new Vendor { Id = 1, UserId = "v-u", Status = VendorStatus.Pending };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _vendorRepoMock.Setup(r => r.Update(It.IsAny<Vendor>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _vendorRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var (success, _) = await _service.UpdateVendorStatusAsync(1, new VendorStatusUpdateDto { Status = "Rejected", RejectionReason = "Invalid tax number" }, "u1");

        Assert.That(success,                Is.True);
        Assert.That(vendor.Status,          Is.EqualTo(VendorStatus.Rejected));
        Assert.That(vendor.RejectionReason, Is.EqualTo("Invalid tax number"));
    }
}
