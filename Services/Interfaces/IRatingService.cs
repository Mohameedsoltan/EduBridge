using EduBridge.Abstractions;
using EduBridge.Contracts.Rating;

namespace EduBridge.Services.Interfaces;

public interface IRatingService
{
    // Queries
    Task<Result<IEnumerable<RatingResponse>>> GetByTaAsync(Guid taId, CancellationToken cancellationToken = default);
    Task<Result<RatingResponse>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<Result<double>> GetAverageAsync(Guid taId, CancellationToken cancellationToken = default);

    // Operations
    Task<Result> SubmitAsync(Guid teamId, SubmitRatingRequest request, CancellationToken cancellationToken = default);
}