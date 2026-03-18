using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Team;

namespace EduBridge.Services.Interfaces;

public interface ITeamService
{
    // Queries
    Task<Result<TeamResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TeamResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    // Team management
    Task<Result<TeamResponse>> CreateAsync(CreateTeamRequest request, CancellationToken cancellationToken = default);
    Task<Result<TeamResponse>> UpdateAsync(Guid id, UpdateTeamRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Members management
    Task<Result> AddMemberAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<Result> RemoveMemberAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<Result> LeaveAsync(Guid id, CancellationToken cancellationToken = default);

    // Status
    Task<Result> ChangeStatusAsync(Guid id, TeamStatus status, CancellationToken cancellationToken = default);

    // Idea
    Task<Result> AssignIdeaAsync(Guid teamId, Guid ideaId, CancellationToken cancellationToken = default);
}