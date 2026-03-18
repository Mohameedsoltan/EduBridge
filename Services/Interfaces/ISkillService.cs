using EduBridge.Abstractions;
using EduBridge.Contracts.Skills;

namespace EduBridge.Services.Interfaces;

public interface ISkillService
{
    Task<Result<IEnumerable<SkillResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<Guid>> GetOrCreateAsync(string skillName, CancellationToken cancellationToken = default);

    // Admin operations
    Task<Result<SkillResponse>> UpdateAsync(Guid id, UpdateSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}