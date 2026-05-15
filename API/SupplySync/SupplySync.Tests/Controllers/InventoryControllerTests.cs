using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Inventory;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class InventoryControllerTests
{
    private Mock<IInventoryService> _serviceMock;
    private InventoryController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IInventoryService>();
        _controller  = new InventoryController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-wm-2") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task IssueItem_WhenSufficientStock_ReturnsOk()
    {
        var dto = new IssueItemDto { InventoryItemId = 1, QuantityIssued = 5, IssuedTo = "Engineering", IssueDate = DateTime.UtcNow };

        _serviceMock.Setup(s => s.IssueItemAsync(dto, "user-wm-2")).ReturnsAsync((true, "Item issued successfully."));

        var result = await _controller.IssueItem(dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task IssueItem_WhenInsufficientStock_ReturnsBadRequest()
    {
        var dto = new IssueItemDto { InventoryItemId = 1, QuantityIssued = 1000 };

        _serviceMock.Setup(s => s.IssueItemAsync(dto, "user-wm-2")).ReturnsAsync((false, "Insufficient stock. Available: 10"));

        var result = await _controller.IssueItem(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task IssueItem_WhenItemNotFound_ReturnsBadRequest()
    {
        var dto = new IssueItemDto { InventoryItemId = 999, QuantityIssued = 1 };

        _serviceMock.Setup(s => s.IssueItemAsync(dto, "user-wm-2")).ReturnsAsync((false, "Inventory item not found."));

        var result = await _controller.IssueItem(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
