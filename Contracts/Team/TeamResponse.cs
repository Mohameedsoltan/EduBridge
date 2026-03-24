using EduBridge.Abstractions.Consts;

namespace EduBridge.Contracts.Team;

public record TeamResponse(
    Guid Id,
    string Name,
    string? Description,
    string LeaderId,
    string LeaderName,
    int MaxMembers,
    int CurrentMembers,
    TeamStatus Status,
    // TA info
    Guid? TaId,
    string? TaName,
    // Doctor info
    Guid? DoctorId,
    string? DoctorName
);