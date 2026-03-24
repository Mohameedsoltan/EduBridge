using EduBridge.Abstractions;
using EduBridge.Contracts.Idea;

namespace EduBridge.Services.Interfaces;

public interface IIdeaCategoryService
{
    Task<Result<IEnumerable<IdeaCategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<Guid>> GetOrCreateAsync(CreateIdeaCategoryRequest request,
        CancellationToken cancellationToken = default);

    // Admin operations
    Task<Result<IdeaCategoryResponse>> UpdateAsync(Guid id, UpdateIdeaCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}