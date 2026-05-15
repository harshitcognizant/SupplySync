using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.DTOs.Auth;
using SupplySync.API.Helpers;
using SupplySync.API.Models;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class AuthControllerTests : IDisposable
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private JwtHelper _jwtHelper;
    private AppDbContext _context;
    private AuthController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory   = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock  = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key",      "supersecrettestkey1234567890abcd" },
                { "Jwt:Issuer",   "TestIssuer"   },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        _jwtHelper  = new JwtHelper(config);
        _controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _jwtHelper, _context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var dto  = new LoginDto { Email = "alice@test.com", Password = "Pass@123" };
        var user = new ApplicationUser { Id = "u1", FullName = "Alice", Email = dto.Email, IsActive = true };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "ProcurementOfficer" });

        var result   = await _controller.Login(dto);
        var ok       = result as OkObjectResult;
        var response = ok!.Value as AuthResponseDto;

        Assert.That(response!.Token, Is.Not.Empty);
        Assert.That(response.Email,  Is.EqualTo("alice@test.com"));
        Assert.That(response.Role,   Is.EqualTo("ProcurementOfficer"));
    }

    [Test]
    public async Task Login_WithInactiveUser_ReturnsUnauthorized()
    {
        var dto  = new LoginDto { Email = "inactive@test.com", Password = "Pass@123" };
        var user = new ApplicationUser { IsActive = false };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(user);

        var result = await _controller.Login(dto);
        Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var dto  = new LoginDto { Email = "alice@test.com", Password = "Wrong" };
        var user = new ApplicationUser { IsActive = true, Email = dto.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _controller.Login(dto);
        Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
    }

    public void Dispose() => _context.Dispose();
}
