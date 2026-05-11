// ============================================================
//  FILE 2 — AuthController
//  Method tested: Login()
//  Fix: JwtHelper.GenerateToken is not virtual — cannot be
//       mocked. Use a real JwtHelper with fake IConfiguration.
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.DTOs.Auth;
using SupplySync.API.Helpers;
using SupplySync.API.Models;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly JwtHelper _jwtHelper;           // real — not mocked
    private readonly AppDbContext _context;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // ── Real InMemory context ──────────────────────────────────
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        // ── Mocked UserManager ─────────────────────────────────────
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // ── Mocked SignInManager ───────────────────────────────────
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null, null, null, null);

        // ── Real JwtHelper with fake config ───────────────────────
        // GenerateToken is not virtual so Moq cannot mock it.
        // We pass a real IConfiguration with dummy values instead.
        var inMemoryConfig = new Dictionary<string, string?>
        {
            { "Jwt:Key",      "supersecrettestkey1234567890abcd" },
            { "Jwt:Issuer",   "TestIssuer"                       },
            { "Jwt:Audience", "TestAudience"                     }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        _jwtHelper = new JwtHelper(config);

        _controller = new AuthController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _jwtHelper,
            _context);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new LoginDto { Email = "alice@test.com", Password = "Pass@123" };
        var user = new ApplicationUser
        {
            Id = "user-1",
            FullName = "Alice",
            Email = "alice@test.com",
            IsActive = true
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "ProcurementOfficer" });

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponseDto>(ok.Value);

        Assert.False(string.IsNullOrEmpty(response.Token)); // real JWT generated
        Assert.Equal("alice@test.com", response.Email);
        Assert.Equal("ProcurementOfficer", response.Role);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto { Email = "inactive@test.com", Password = "Pass@123" };
        var user = new ApplicationUser { IsActive = false };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto { Email = "alice@test.com", Password = "WrongPass" };
        var user = new ApplicationUser { IsActive = true, Email = dto.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    public void Dispose() => _context.Dispose();
}