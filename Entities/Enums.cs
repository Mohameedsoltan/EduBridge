namespace EduBridge.Enums;

public enum TeamStatus
{
    Open,
    Partial,
    Full,
    IdeaSelection,
    TaPending,
    TaApproved,
    InProgress,
    Completed,
    Closed
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public enum MemberRole
{
    Leader,
    Member
}

public enum NotificationType
{
    JoinRequestReceived,
    JoinRequestAccepted,
    JoinRequestRejected,
    TARequestReceived,
    TARequestAccepted,
    TARequestRejected,
    TeamMemberJoined,
    RatingReceived,
    TeamInviteReceived,
    TeamInviteAccepted,
    TeamInviteRejected
}
