using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.Models;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class AdminControllerTests : IDisposable
{
    private AppDbContext _context;
    private UserManager<ApplicationUser> _userManager;
    private AdminController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var userStore    = new UserStore<ApplicationUser>(_context);
        var hasher       = new PasswordHasher<ApplicationUser>();
        var validators   = new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
        var pwValidators = new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() };

        _userManager = new UserManager<ApplicationUser>(
            userStore, null, hasher, validators, pwValidators,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), null, null);

        _controller = new AdminController(_userManager);
    }

    [TearDown]
    public void TearDown()
    {
        _userManager.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task GetAllUsers_ReturnsOkWithUserList()
    {
        await _userManager.CreateAsync(new ApplicationUser { Id = "u1", FullName = "Alice", Email = "alice@test.com", UserName = "alice@test.com", IsActive = true }, "Pass@1234");
        await _userManager.CreateAsync(new ApplicationUser { Id = "u2", FullName = "Bob",   Email = "bob@test.com",   UserName = "bob@test.com",   IsActive = true }, "Pass@1234");

        var result = await _controller.GetAllUsers();

        var ok   = result as OkObjectResult;
        var list = ok!.Value as List<object>;
        Assert.That(list!.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllUsers_WhenNoUsers_ReturnsEmptyList()
    {
        var result = await _controller.GetAllUsers();

        var ok   = result as OkObjectResult;
        var list = ok!.Value as List<object>;
        Assert.That(list!, Is.Empty);
    }

    public void Dispose() { _userManager.Dispose(); _context.Dispose(); }
}
