using EduBridge.Abstractions;
using EduBridge.Contracts.TA;
using EduBridge.Contracts.Team;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class TaService(
    ApplicationDbContext context,
    IRatingService ratingService,
    IMapper mapper) : ITaService
{
    public async Task<Result<IEnumerable<TAResponse>>> GetAllTAsAsync(
        CancellationToken cancellationToken = default)
    {
        var tas = await context.TeachingAssistants
            .AsNoTracking()
            .Include(ta => ta.User)
            .ToListAsync(cancellationToken);

        return Result.Success(await MapWithRatingsAsync(tas, cancellationToken));
    }

    public async Task<Result<IEnumerable<TAResponse>>> GetAvailableTAsAsync(
        CancellationToken cancellationToken = default)
    {
        var tas = await context.TeachingAssistants
            .AsNoTracking()
            .Include(ta => ta.User)
            .Where(ta => ta.AvailableSlots > 0)
            .ToListAsync(cancellationToken);

        return Result.Success(await MapWithRatingsAsync(tas, cancellationToken));
    }

    public async Task<Result<IEnumerable<TeamResponse>>> GetSupervisedTeamsAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var ta = await context.TeachingAssistants
            .FirstOrDefaultAsync(ta => ta.UserId == userId, cancellationToken);

        if (ta is null)
            return Result.Failure<IEnumerable<TeamResponse>>(TaErrors.TaNotFound);

        var teams = await context.Teams
            .AsNoTracking()
            .Include(t => t.Members)
            .Include(t => t.Leader)
            .Where(t => t.TaId == ta.Id)
            .ToListAsync(cancellationToken);

        return Result.Success(mapper.Map<IEnumerable<TeamResponse>>(teams));
    }

    private async Task<IEnumerable<TAResponse>> MapWithRatingsAsync(
        IEnumerable<TeachingAssistant> tas, CancellationToken cancellationToken)
    {
        var response = new List<TAResponse>();

        foreach (var ta in tas)
        {
            var avgResult = await ratingService.GetAverageAsync(ta.Id, cancellationToken);

            response.Add(new TAResponse(
                ta.UserId,
                ta.User.FirstName,
                ta.User.LastName,
                ta.Department,
                ta.AcademicTitle,
                ta.OfficeLocation,
                ta.MaxSlots,
                ta.AvailableSlots,
                ta.IsAvailable,
                avgResult.IsSuccess ? avgResult.Value : 0
            ));
        }

        return response;
    }
}