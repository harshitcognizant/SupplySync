// ============================================================
//  FILE 8 — NotificationController
//  Method tested: MarkAsRead()
// ============================================================

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupplySync.API.Controllers;
using SupplySync.API.Interfaces;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _serviceMock;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _serviceMock = new Mock<INotificationService>();
        _controller = new NotificationController(_serviceMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-1") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task MarkAsRead_WhenNotificationBelongsToUser_ReturnsOk()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.MarkAsReadAsync(5, "user-1"))
            .ReturnsAsync((true, "Notification marked as read."));

        // Act
        var result = await _controller.MarkAsRead(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task MarkAsRead_WhenNotificationNotFound_ReturnsBadRequest()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.MarkAsReadAsync(999, "user-1"))
            .ReturnsAsync((false, "Notification not found."));

        // Act
        var result = await _controller.MarkAsRead(999);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task MarkAsRead_WhenUnauthorizedUser_ReturnsBadRequest()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.MarkAsReadAsync(5, "user-1"))
            .ReturnsAsync((false, "Unauthorized."));

        // Act
        var result = await _controller.MarkAsRead(5);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}