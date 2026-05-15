using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.DTOs.Contract;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class ContractControllerTests
{
    private Mock<IContractService> _serviceMock;
    private ContractController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IContractService>();
        _controller  = new ContractController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-po-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task Create_WhenVendorApprovedAndValid_ReturnsOk()
    {
        var dto = new CreateContractDto { VendorId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1), PaymentTerms = "Net 30", DeliveryTerms = "FOB", ItemPricing = "Fixed" };
        var contractDto = new ContractDto { Id = 10, ContractNumber = "CON-12345678", VendorName = "Acme", Status = "Draft" };

        _serviceMock.Setup(s => s.CreateContractAsync(dto, "user-po-1")).ReturnsAsync((true, "Contract created successfully.", contractDto));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task Create_WhenVendorNotApproved_ReturnsBadRequest()
    {
        var dto = new CreateContractDto { VendorId = 2 };

        _serviceMock.Setup(s => s.CreateContractAsync(dto, "user-po-1")).ReturnsAsync((false, "Vendor must be approved before creating a contract.", null));

        var result = await _controller.Create(dto);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
