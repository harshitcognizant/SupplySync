using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Vendor;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class VendorControllerTests
{
    private Mock<IVendorService> _serviceMock;
    private VendorController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IVendorService>();
        _controller  = new VendorController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task UpdateStatus_WhenApproved_ReturnsOk()
    {
        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        _serviceMock.Setup(s => s.UpdateVendorStatusAsync(1, dto, "user-po-1")).ReturnsAsync((true, "Vendor status updated to Approved."));

        var result = await _controller.UpdateStatus(1, dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task UpdateStatus_WhenRejected_ReturnsOk()
    {
        var dto = new VendorStatusUpdateDto { Status = "Rejected", RejectionReason = "Invalid license" };

        _serviceMock.Setup(s => s.UpdateVendorStatusAsync(1, dto, "user-po-1")).ReturnsAsync((true, "Vendor status updated to Rejected."));

        var result = await _controller.UpdateStatus(1, dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task UpdateStatus_WhenVendorNotFound_ReturnsBadRequest()
    {
        var dto = new VendorStatusUpdateDto { Status = "Approved" };

        _serviceMock.Setup(s => s.UpdateVendorStatusAsync(999, dto, "user-po-1")).ReturnsAsync((false, "Vendor not found."));

        var result = await _controller.UpdateStatus(999, dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
