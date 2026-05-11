// ============================================================
//  FILE 3 — ComplianceController
//  Method tested: PerformCheck()
// ============================================================

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Compliance;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class ComplianceControllerTests
{
    private readonly Mock<IComplianceService> _serviceMock;
    private readonly ComplianceController _controller;

    public ComplianceControllerTests()
    {
        _serviceMock = new Mock<IComplianceService>();
        _controller = new ComplianceController(_serviceMock.Object);

        // Inject a fake authenticated user into HttpContext
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-co-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task PerformCheck_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var dto = new CreateComplianceCheckDto
        {
            EntityType = "Vendor",
            EntityId = 5,
            Status = "Pass",
            Remarks = "All documents verified."
        };

        var resultDto = new ComplianceCheckDto
        {
            Id = 1,
            EntityType = "Vendor",
            EntityId = 5,
            Status = "Pass",
            Remarks = "All documents verified."
        };

        _serviceMock
            .Setup(s => s.PerformCheckAsync(dto, "user-co-1"))
            .ReturnsAsync((true, "Compliance check recorded.", resultDto));

        // Act
        var result = await _controller.PerformCheck(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task PerformCheck_WhenInvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateComplianceCheckDto
        {
            EntityType = "Vendor",
            EntityId = 5,
            Status = "Unknown"
        };

        _serviceMock
            .Setup(s => s.PerformCheckAsync(dto, "user-co-1"))
            .ReturnsAsync((false, "Invalid status. Use Pass or Fail.", null));

        // Act
        var result = await _controller.PerformCheck(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}