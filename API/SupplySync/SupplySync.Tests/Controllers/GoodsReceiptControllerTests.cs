using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.GoodsReceipt;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class GoodsReceiptControllerTests
{
    private Mock<IGoodsReceiptService> _serviceMock;
    private GoodsReceiptController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IGoodsReceiptService>();
        _controller  = new GoodsReceiptController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-wm-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task Create_WhenPODelivered_ReturnsOk()
    {
        var dto = new CreateGoodsReceiptDto
        {
            PurchaseOrderId = 1,
            Remarks = "All good",
            Status  = "Accepted",
            Items   = new List<GoodsReceiptItemDto> { new GoodsReceiptItemDto { ItemName = "Laptop", ReceivedQuantity = 10, Condition = "Good" } }
        };
        var grDto = new GoodsReceiptDto { Id = 1, GRNumber = "GR-001", PurchaseOrderId = 1, Status = "Accepted" };

        _serviceMock.Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1")).ReturnsAsync((true, "Goods receipt created successfully.", grDto));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Create_WhenPONotDelivered_ReturnsBadRequest()
    {
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 99 };

        _serviceMock.Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1")).ReturnsAsync((false, "Cannot receive goods. Vendor has not marked this PO as Delivered yet.", null));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Create_WhenDuplicateGR_ReturnsBadRequest()
    {
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 1 };

        _serviceMock.Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1")).ReturnsAsync((false, "Goods Receipt already exists for this Purchase Order. Cannot receive the same PO twice.", null));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
