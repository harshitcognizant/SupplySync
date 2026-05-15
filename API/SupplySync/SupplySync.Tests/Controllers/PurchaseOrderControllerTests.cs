using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.PurchaseOrder;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class PurchaseOrderControllerTests
{
    private Mock<IPurchaseOrderService> _serviceMock;
    private PurchaseOrderController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IPurchaseOrderService>();
        _controller  = new PurchaseOrderController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task Create_WithActiveContract_ReturnsOk()
    {
        var dto = new CreatePurchaseOrderDto
        {
            VendorId = 1, ContractId = 2,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(30),
            Items = new List<CreatePOItemDto> { new CreatePOItemDto { ItemName = "Monitor", Quantity = 5, UnitPrice = 200m } }
        };
        var poDto = new PurchaseOrderDto { Id = 1, PONumber = "PO-001", Status = "Sent" };

        _serviceMock.Setup(s => s.CreatePOAsync(dto, "user-po-1")).ReturnsAsync((true, "Purchase Order created and sent to vendor.", poDto));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Create_WithInactiveContract_ReturnsBadRequest()
    {
        var dto = new CreatePurchaseOrderDto { ContractId = 99 };

        _serviceMock.Setup(s => s.CreatePOAsync(dto, "user-po-1")).ReturnsAsync((false, "An active contract is required to create a PO.", null));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
