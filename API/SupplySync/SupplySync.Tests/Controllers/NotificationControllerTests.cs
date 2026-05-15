using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.Interfaces;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class NotificationControllerTests
{
    private Mock<INotificationService> _serviceMock;
    private NotificationController _controller;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<INotificationService>();
        _controller  = new NotificationController(_serviceMock.Object);

        var claims   = new[] { new Claim(ClaimTypes.NameIdentifier, "user-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public async Task MarkAsRead_WhenValid_ReturnsOk()
    {
        _serviceMock.Setup(s => s.MarkAsReadAsync(5, "user-1")).ReturnsAsync((true, "Notification marked as read."));

        var result = await _controller.MarkAsRead(5);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task MarkAsRead_WhenNotFound_ReturnsBadRequest()
    {
        _serviceMock.Setup(s => s.MarkAsReadAsync(999, "user-1")).ReturnsAsync((false, "Notification not found."));

        var result = await _controller.MarkAsRead(999);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task MarkAsRead_WhenUnauthorized_ReturnsBadRequest()
    {
        _serviceMock.Setup(s => s.MarkAsReadAsync(5, "user-1")).ReturnsAsync((false, "Unauthorized."));

        var result = await _controller.MarkAsRead(5);
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
}
