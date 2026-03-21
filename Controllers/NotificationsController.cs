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
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("")]
    public async Task<IActionResult> GetUserNotificationsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching notifications for user {UserId}", CurrentUserId);

        var result = await notificationService.GetUserNotificationsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCountAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching unread notification count for user {UserId}", CurrentUserId);

        var result = await notificationService.GetUnreadCountAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsReadAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking notification {NotificationId} as read", id);

        var result = await notificationService.MarkAsReadAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsReadAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking all notifications as read for user {UserId}", CurrentUserId);

        var result = await notificationService.MarkAllAsReadAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}