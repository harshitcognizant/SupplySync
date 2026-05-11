// ============================================================
//  FILE 18 — VendorService
//  Method tested: UpdateVendorStatusAsync()
// ============================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using SupplySync.API.DTOs.Vendor;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class VendorServiceTests
{
    private readonly Mock<IVendorRepository> _vendorRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VendorService _service;

    public VendorServiceTests()
    {
        _vendorRepoMock = new Mock<IVendorRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _service = new VendorService(
            _vendorRepoMock.Object,
            _notificationRepoMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task UpdateVendorStatusAsync_WhenVendorNotFound_ReturnsFailure()
    {
        // Arrange
        _vendorRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Vendor?)null);

        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        // Act
        var (success, message) =
            await _service.UpdateVendorStatusAsync(99, dto, "user-po-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Vendor not found.", message);
    }

    [Fact]
    public async Task UpdateVendorStatusAsync_WhenInvalidStatus_ReturnsFailure()
    {
        // Arrange
        var vendor = new Vendor { Id = 1, UserId = "vendor-user" };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);

        var dto = new VendorStatusUpdateDto { Status = "InvalidStatus" };

        // Act
        var (success, message) =
            await _service.UpdateVendorStatusAsync(1, dto, "user-po-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Invalid status value.", message);
    }

    [Fact]
    public async Task UpdateVendorStatusAsync_WhenApproved_UpdatesAndNotifies()
    {
        // Arrange
        var vendor = new Vendor
        {
            Id = 1,
            UserId = "vendor-user",
            CompanyName = "Acme Corp",
            Status = VendorStatus.Pending
        };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _vendorRepoMock.Setup(r => r.Update(It.IsAny<Vendor>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _vendorRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        // Act
        var (success, message) =
            await _service.UpdateVendorStatusAsync(1, dto, "user-po-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Vendor status updated to Approved.", message);
        Assert.Equal(VendorStatus.Approved, vendor.Status);

        _notificationRepoMock.Verify(
            r => r.AddAsync(It.Is<Notification>(n => n.Type == "VendorStatusUpdate")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVendorStatusAsync_WhenRejected_SetsRejectionReason()
    {
        // Arrange
        var vendor = new Vendor
        {
            Id = 1,
            UserId = "vendor-user",
            Status = VendorStatus.Pending
        };

        _vendorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vendor);
        _vendorRepoMock.Setup(r => r.Update(It.IsAny<Vendor>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _vendorRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var dto = new VendorStatusUpdateDto
        {
            Status = "Rejected",
            RejectionReason = "Invalid tax number"
        };

        // Act
        var (success, _) =
            await _service.UpdateVendorStatusAsync(1, dto, "user-po-1");

        // Assert
        Assert.True(success);
        Assert.Equal(VendorStatus.Rejected, vendor.Status);
        Assert.Equal("Invalid tax number", vendor.RejectionReason);
    }
}