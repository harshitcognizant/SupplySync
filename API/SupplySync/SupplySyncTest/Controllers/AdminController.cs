// ============================================================
//  FILE 1 — AdminController
//  Method tested: GetAllUsers()
//  Fix: Use InMemory database instead of mocking IQueryable
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.Models;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class AdminControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        // InMemory database supports IAsyncEnumerable — fixes the error
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        var userStore = new UserStore<ApplicationUser>(_context);
        var hasher = new PasswordHasher<ApplicationUser>();
        var validators = new List<IUserValidator<ApplicationUser>>
                           { new UserValidator<ApplicationUser>() };
        var pwValidators = new List<IPasswordValidator<ApplicationUser>>
                           { new PasswordValidator<ApplicationUser>() };

        _userManager = new UserManager<ApplicationUser>(
            userStore,
            null,
            hasher,
            validators,
            pwValidators,
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            null
        );

        _controller = new AdminController(_userManager);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkWithUserList()
    {
        // Arrange
        await _userManager.CreateAsync(new ApplicationUser
        {
            Id = "user-1",
            FullName = "Alice Admin",
            Email = "alice@test.com",
            UserName = "alice@test.com",
            IsActive = true
        }, "Pass@1234");

        await _userManager.CreateAsync(new ApplicationUser
        {
            Id = "user-2",
            FullName = "Bob Officer",
            Email = "bob@test.com",
            UserName = "bob@test.com",
            IsActive = true
        }, "Pass@1234");

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<object>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllUsers_WhenNoUsers_ReturnsEmptyList()
    {
        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<object>>(ok.Value);
        Assert.Empty(list);
    }

    public void Dispose()
    {
        _userManager.Dispose();
        _context.Dispose();
    }
}