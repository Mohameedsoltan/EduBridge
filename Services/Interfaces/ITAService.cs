using EduBridge.Abstractions;
using EduBridge.Contracts.TA;
using EduBridge.Contracts.Team;

namespace EduBridge.Services.Interfaces;

public interface ITaService
{
    // Queries
    Task<Result<IEnumerable<TAResponse>>> GetAllTAsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TAResponse>>> GetAvailableTAsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TeamResponse>>> GetSupervisedTeamsAsync(string userId, CancellationToken cancellationToken = default);
}