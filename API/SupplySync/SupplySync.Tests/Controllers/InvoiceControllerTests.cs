using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Invoice;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class InvoiceControllerTests
{
    private Mock<IInvoiceService> _serviceMock;
    private InvoiceController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _controller  = new InvoiceController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-fo-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task Review_WhenApproved_ReturnsOk()
    {
        var dto = new InvoiceReviewDto { Status = "Approved" };

        _serviceMock.Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1")).ReturnsAsync((true, "Invoice Approved successfully."));

        var result = await _controller.Review(1, dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Review_WhenRejected_ReturnsOk()
    {
        var dto = new InvoiceReviewDto { Status = "Rejected", RejectionReason = "Amount mismatch" };

        _serviceMock.Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1")).ReturnsAsync((true, "Invoice Rejected successfully."));

        var result = await _controller.Review(1, dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Review_WhenGRRejected_ReturnsBadRequest()
    {
        var dto = new InvoiceReviewDto { Status = "Approved" };

        _serviceMock.Setup(s => s.ReviewInvoiceAsync(1, dto, "user-fo-1")).ReturnsAsync((false, "Cannot approve invoice. Warehouse Manager rejected the delivery."));

        var result = await _controller.Review(1, dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
