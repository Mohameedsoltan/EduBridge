using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.Skills;

public record JoinRequestResponse(
    Guid Id,
    Guid TeamId,
    string TeamName,
    string StudentName,
    RequestStatus Status,
    string? Message,
    string? ResponseMessage,
    DateTime CreatedAt,
    DateTime? RespondedAt
);