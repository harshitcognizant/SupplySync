// ============================================================
//  FILE 10 — VendorController
//  Method tested: UpdateStatus()
// ============================================================

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Vendor;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class VendorControllerTests
{
    private readonly Mock<IVendorService> _serviceMock;
    private readonly VendorController _controller;

    public VendorControllerTests()
    {
        _serviceMock = new Mock<IVendorService>();
        _controller = new VendorController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task UpdateStatus_WhenApproved_ReturnsOk()
    {
        // Arrange
        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        _serviceMock
            .Setup(s => s.UpdateVendorStatusAsync(1, dto, "user-po-1"))
            .ReturnsAsync((true, "Vendor status updated to Approved."));

        // Act
        var result = await _controller.UpdateStatus(1, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task UpdateStatus_WhenRejected_ReturnsOk()
    {
        // Arrange
        var dto = new VendorStatusUpdateDto
        {
            Status = "Rejected",
            RejectionReason = "Invalid license number"
        };

        _serviceMock
            .Setup(s => s.UpdateVendorStatusAsync(1, dto, "user-po-1"))
            .ReturnsAsync((true, "Vendor status updated to Rejected."));

        // Act
        var result = await _controller.UpdateStatus(1, dto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_WhenVendorNotFound_ReturnsBadRequest()
    {
        // Arrange
        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        _serviceMock
            .Setup(s => s.UpdateVendorStatusAsync(999, dto, "user-po-1"))
            .ReturnsAsync((false, "Vendor not found."));

        // Act
        var result = await _controller.UpdateStatus(999, dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}