using EduBridge.Abstractions;
using EduBridge.Contracts.TA;
using EduBridge.Contracts.Team;

namespace EduBridge.Services.Interfaces;

public interface ITaService
{
    // Queries
    Task<Result<IEnumerable<TAResponse>>> GetAllTAsAsync(CancellationToken cancellationToken = default);
    Task<Result<TAResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TAResponse>>> GetAvailableTAsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TeamResponse>>> GetSupervisedTeamsAsync(CancellationToken cancellationToken = default);

    // Commands
    Task<Result> CreateAsync(string currentUserId, CreateTaRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid id, UpdateTaRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}