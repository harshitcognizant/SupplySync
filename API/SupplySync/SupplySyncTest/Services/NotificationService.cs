// ============================================================
//  FILE 16 — NotificationService
//  Method tested: MarkAsReadAsync()
// ============================================================

using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new NotificationService(
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenNotificationNotFound_ReturnsFailure()
    {
        // Arrange
        _notificationRepoMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Notification?)null);

        // Act
        var (success, message) = await _service.MarkAsReadAsync(99, "user-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Notification not found.", message);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenWrongUser_ReturnsUnauthorized()
    {
        // Arrange
        var notification = new Notification
        {
            Id = 5,
            UserId = "user-2",   // different user
            IsRead = false
        };

        _notificationRepoMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(notification);

        // Act
        var (success, message) = await _service.MarkAsReadAsync(5, "user-1");

        // Assert
        Assert.False(success);
        Assert.Equal("Unauthorized.", message);
    }

    [Fact]
    public async Task MarkAsReadAsync_WhenValidUser_MarksAndSaves()
    {
        // Arrange
        var notification = new Notification
        {
            Id = 5,
            UserId = "user-1",
            IsRead = false
        };

        _notificationRepoMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(notification);
        _notificationRepoMock.Setup(r => r.Update(It.IsAny<Notification>()));
        _notificationRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var (success, message) = await _service.MarkAsReadAsync(5, "user-1");

        // Assert
        Assert.True(success);
        Assert.Equal("Notification marked as read.", message);
        Assert.True(notification.IsRead);

        _notificationRepoMock.Verify(
            r => r.Update(It.Is<Notification>(n => n.IsRead)), Times.Once);
        _notificationRepoMock.Verify(r => r.SaveAsync(), Times.Once);
    }
}