using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.Team;

public record TeamResponse(
    Guid Id,
    string Name,
    string? Description,
    string LeaderName,
    int MaxMembers,
    int CurrentMembers,
    TeamStatus Status,
    DateTime CreatedAt
);