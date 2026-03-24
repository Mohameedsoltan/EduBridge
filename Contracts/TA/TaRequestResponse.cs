using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.TA;

public record TaRequestResponse(
    Guid Id,
    Guid TeamId,
    string TeamName,
    string TaName,
    string? Department,
    RequestStatus Status,
    string? Message,
    string? ResponseMessage,
    DateTime CreatedAt,
    DateTime? RespondedAt
);