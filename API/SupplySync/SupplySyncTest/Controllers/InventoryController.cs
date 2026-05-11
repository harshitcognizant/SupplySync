// ============================================================
//  FILE 6 — InventoryController
//  Method tested: IssueItem()
// ============================================================

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Inventory;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class InventoryControllerTests
{
    private readonly Mock<IInventoryService> _serviceMock;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _serviceMock = new Mock<IInventoryService>();
        _controller = new InventoryController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-wm-2") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task IssueItem_WhenSufficientStock_ReturnsOk()
    {
        // Arrange
        var dto = new IssueItemDto
        {
            InventoryItemId = 1,
            QuantityIssued = 5,
            IssuedTo = "Engineering Dept",
            IssueDate = DateTime.UtcNow,
            Remarks = "Monthly issuance"
        };

        _serviceMock
            .Setup(s => s.IssueItemAsync(dto, "user-wm-2"))
            .ReturnsAsync((true, "Item issued successfully."));

        // Act
        var result = await _controller.IssueItem(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task IssueItem_WhenInsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var dto = new IssueItemDto
        {
            InventoryItemId = 1,
            QuantityIssued = 1000
        };

        _serviceMock
            .Setup(s => s.IssueItemAsync(dto, "user-wm-2"))
            .ReturnsAsync((false, "Insufficient stock. Available: 10"));

        // Act
        var result = await _controller.IssueItem(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task IssueItem_WhenItemNotFound_ReturnsBadRequest()
    {
        // Arrange
        var dto = new IssueItemDto { InventoryItemId = 999, QuantityIssued = 1 };

        _serviceMock
            .Setup(s => s.IssueItemAsync(dto, "user-wm-2"))
            .ReturnsAsync((false, "Inventory item not found."));

        // Act
        var result = await _controller.IssueItem(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}