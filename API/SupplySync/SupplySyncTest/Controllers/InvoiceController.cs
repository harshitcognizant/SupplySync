// ============================================================
//  FILE 7 — InvoiceController
//  Method tested: Review()
// ============================================================

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Invoice;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class InvoiceControllerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly InvoiceController _controller;

    public InvoiceControllerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _controller = new InvoiceController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-fo-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Review_WhenApproved_ReturnsOk()
    {
        // Arrange
        var dto = new InvoiceReviewDto { Status = "Approved" };

        _serviceMock
            .Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1"))
            .ReturnsAsync((true, "Invoice Approved successfully."));

        // Act
        var result = await _controller.Review(1, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Review_WhenRejected_ReturnsOk()
    {
        // Arrange
        var dto = new InvoiceReviewDto
        {
            Status = "Rejected",
            RejectionReason = "Amount mismatch"
        };

        _serviceMock
            .Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1"))
            .ReturnsAsync((true, "Invoice Rejected successfully."));

        // Act
        var result = await _controller.Review(1, dto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Review_WhenGRRejected_ReturnsBadRequest()
    {
        // Arrange
        var dto = new InvoiceReviewDto { Status = "Approved" };

        _serviceMock
            .Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1"))
            .ReturnsAsync((false,
                "Cannot approve invoice. Warehouse Manager rejected the delivery."));

        // Act
        var result = await _controller.Review(1, dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}