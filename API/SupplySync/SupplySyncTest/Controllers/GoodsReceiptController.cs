// ============================================================
//  FILE 5 — GoodsReceiptController
//  Method tested: Create()
// ============================================================

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.GoodsReceipt;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class GoodsReceiptControllerTests
{
    private readonly Mock<IGoodsReceiptService> _serviceMock;
    private readonly GoodsReceiptController _controller;

    public GoodsReceiptControllerTests()
    {
        _serviceMock = new Mock<IGoodsReceiptService>();
        _controller = new GoodsReceiptController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-wm-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Create_WhenPODelivered_ReturnsOk()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto
        {
            PurchaseOrderId = 1,
            Remarks = "All good",
            Status = "Accepted",
            Items = new List<GoodsReceiptItemDto>
            {
                new GoodsReceiptItemDto
                {
                    ItemName         = "Laptop",
                    ReceivedQuantity = 10,
                    Condition        = "Good"
                }
            }
        };

        var grDto = new GoodsReceiptDto
        {
            Id = 1,
            GRNumber = "GR-00000001",
            PurchaseOrderId = 1,
            Status = "Accepted"
        };

        _serviceMock
            .Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1"))
            .ReturnsAsync((true, "Goods receipt created successfully.", grDto));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Create_WhenPONotDelivered_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 99 };

        _serviceMock
            .Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1"))
            .ReturnsAsync((false,
                "Cannot receive goods. Vendor has not marked this PO as Delivered yet.",
                null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_WhenDuplicateGR_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateGoodsReceiptDto { PurchaseOrderId = 1 };

        _serviceMock
            .Setup(s => s.CreateGoodsReceiptAsync(dto, "user-wm-1"))
            .ReturnsAsync((false,
                "Goods Receipt already exists for this Purchase Order. Cannot receive the same PO twice.",
                null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}