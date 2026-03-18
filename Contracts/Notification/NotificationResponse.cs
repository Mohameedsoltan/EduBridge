using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.Notification;

public record NotificationResponse(
    Guid Id,
    string Message,
    NotificationType Type,
    bool IsRead,
    Guid? RelatedEntityId,
    DateTime CreatedAt
);