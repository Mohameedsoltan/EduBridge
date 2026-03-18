using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.Team;

public record TeamMemberResponse(
    string UserId,
    string FullName,
    string? ProfileImageUrl,
    MemberRole Role,
    DateTime JoinedAt
);