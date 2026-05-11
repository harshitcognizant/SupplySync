// ============================================================
//  FILE 4 — ContractController
//  Method tested: Create()
// ============================================================

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Contract;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class ContractControllerTests
{
    private readonly Mock<IContractService> _serviceMock;
    private readonly ContractController _controller;

    public ContractControllerTests()
    {
        _serviceMock = new Mock<IContractService>();
        _controller = new ContractController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Create_WhenVendorApprovedAndValid_ReturnsOk()
    {
        // Arrange
        var dto = new CreateContractDto
        {
            VendorId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            PaymentTerms = "Net 30",
            DeliveryTerms = "FOB",
            ItemPricing = "Fixed"
        };

        var contractDto = new ContractDto
        {
            Id = 10,
            ContractNumber = "CON-12345678",
            VendorId = 1,
            VendorName = "Acme Corp",
            Status = "Draft"
        };

        _serviceMock
            .Setup(s => s.CreateContractAsync(dto, "user-po-1"))
            .ReturnsAsync((true, "Contract created successfully.", contractDto));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Create_WhenVendorNotApproved_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateContractDto { VendorId = 2 };

        _serviceMock
            .Setup(s => s.CreateContractAsync(dto, "user-po-1"))
            .ReturnsAsync((false, "Vendor must be approved before creating a contract.", null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}