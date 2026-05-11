// ============================================================
//  FILE 9 — PurchaseOrderController
//  Method tested: Create()
// ============================================================

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.PurchaseOrder;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class PurchaseOrderControllerTests
{
    private readonly Mock<IPurchaseOrderService> _serviceMock;
    private readonly PurchaseOrderController _controller;

    public PurchaseOrderControllerTests()
    {
        _serviceMock = new Mock<IPurchaseOrderService>();
        _controller = new PurchaseOrderController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Create_WithActiveContract_ReturnsOk()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto
        {
            VendorId = 1,
            ContractId = 2,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new List<CreatePOItemDto>
            {
                new CreatePOItemDto
                {
                    ItemName  = "Monitor",
                    Quantity  = 5,
                    UnitPrice = 200m
                }
            }
        };

        var poDto = new PurchaseOrderDto
        {
            Id = 1,
            PONumber = "PO-12345678",
            Status = "Sent"
        };

        _serviceMock
            .Setup(s => s.CreatePOAsync(dto, "user-po-1"))
            .ReturnsAsync((true, "Purchase Order created and sent to vendor.", poDto));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Create_WithInactiveContract_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePurchaseOrderDto { ContractId = 99 };

        _serviceMock
            .Setup(s => s.CreatePOAsync(dto, "user-po-1"))
            .ReturnsAsync((false, "An active contract is required to create a PO.", null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}