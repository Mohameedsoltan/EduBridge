using EduBridge.Abstractions;
using EduBridge.Contracts.Notification;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class NotificationsController(
    INotificationService notificationService,
    ILogger<NotificationsController> logger) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationsController> _logger = logger;

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("")]
    public async Task<IActionResult> GetUserNotificationsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching notifications for user {UserId}", CurrentUserId);

        var result = await _notificationService.GetUserNotificationsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCountAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching unread notification count for user {UserId}", CurrentUserId);

        var result = await _notificationService.GetUnreadCountAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsReadAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking notification {NotificationId} as read", id);

        var result = await _notificationService.MarkAsReadAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsReadAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking all notifications as read for user {UserId}", CurrentUserId);

        var result = await _notificationService.MarkAllAsReadAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}