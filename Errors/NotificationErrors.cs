using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class NotificationErrors
{
    public static readonly Error NotificationNotFound = new(
        "Notification.NotFound", "Notification not found", StatusCodes.Status404NotFound);
}