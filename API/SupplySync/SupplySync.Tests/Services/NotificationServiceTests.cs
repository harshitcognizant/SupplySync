using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class NotificationServiceTests
{
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<IMapper> _mapperMock;
    private NotificationService _service;

    [SetUp]
    public void SetUp()
    {
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();
        _service              = new NotificationService(_notificationRepoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task MarkAsReadAsync_WhenNotFound_ReturnsFailure()
    {
        _notificationRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Notification?)null);

        var (success, message) = await _service.MarkAsReadAsync(99, "user-1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Notification not found."));
    }

    [Test]
    public async Task MarkAsReadAsync_WhenWrongUser_ReturnsUnauthorized()
    {
        _notificationRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Notification { Id = 5, UserId = "user-2", IsRead = false });

        var (success, message) = await _service.MarkAsReadAsync(5, "user-1");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Unauthorized."));
    }

    [Test]
    public async Task MarkAsReadAsync_WhenValid_MarksAndSaves()
    {
        var notification = new Notification { Id = 5, UserId = "user-1", IsRead = false };

        _notificationRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(notification);
        _notificationRepoMock.Setup(r => r.Update(It.IsAny<Notification>()));
        _notificationRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var (success, message) = await _service.MarkAsReadAsync(5, "user-1");

        Assert.That(success,           Is.True);
        Assert.That(message,           Is.EqualTo("Notification marked as read."));
        Assert.That(notification.IsRead, Is.True);
        _notificationRepoMock.Verify(r => r.Update(It.Is<Notification>(n => n.IsRead)), Times.Once);
        _notificationRepoMock.Verify(r => r.SaveAsync(), Times.Once);
    }
}
