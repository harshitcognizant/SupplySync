using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Compliance;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class ComplianceControllerTests
{
    private Mock<IComplianceService> _serviceMock;
    private ComplianceController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IComplianceService>();
        _controller  = new ComplianceController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-co-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task PerformCheck_WhenSuccess_ReturnsOk()
    {
        var dto = new CreateComplianceCheckDto { EntityType = "Vendor", EntityId = 5, Status = "Pass", Remarks = "Verified." };
        var resultDto = new ComplianceCheckDto { Id = 1, Status = "Pass" };

        _serviceMock.Setup(s => s.PerformCheckAsync(dto, "user-co-1")).ReturnsAsync((true, "Compliance check recorded.", resultDto));

        var result = await _controller.PerformCheck(dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task PerformCheck_WhenInvalidStatus_ReturnsBadRequest()
    {
        var dto = new CreateComplianceCheckDto { EntityType = "Vendor", EntityId = 5, Status = "Unknown" };

        _serviceMock.Setup(s => s.PerformCheckAsync(dto, "user-co-1")).ReturnsAsync((false, "Invalid status. Use Pass or Fail.", null));

        var result = await _controller.PerformCheck(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
