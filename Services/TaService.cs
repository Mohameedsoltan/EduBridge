using System.Security.Claims;
using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.TA;
using EduBridge.Contracts.Team;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class TaService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IRatingService ratingService,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : ITaService
{
    private string CurrentUserId => httpContextAccessor.HttpContext!.User
        .FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<Result<IEnumerable<TAResponse>>> GetAllTAsAsync(
        CancellationToken cancellationToken = default)
    {
        var tas = await context.TeachingAssistants
            .AsNoTracking()
            .Include(ta => ta.User)
            .ToListAsync(cancellationToken);

        return Result.Success(await MapWithRatingsAsync(tas, cancellationToken));
    }

    public async Task<Result<TAResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var ta = await context.TeachingAssistants
            .AsNoTracking()
            .Include(ta => ta.User)
            .FirstOrDefaultAsync(ta => ta.Id == id, cancellationToken);

        if (ta is null)
            return Result.Failure<TAResponse>(TaErrors.TaNotFound);

        var avgResult = await ratingService.GetAverageAsync(ta.Id, cancellationToken);
        var response = mapper.Map<TAResponse>(ta) with { AverageRating = avgResult.IsSuccess ? avgResult.Value : 0 };

        return Result.Success(response);
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
        CancellationToken cancellationToken = default)
    {
        var ta = await context.TeachingAssistants
            .FirstOrDefaultAsync(ta => ta.UserId == CurrentUserId, cancellationToken);

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

    public async Task<Result> CreateAsync(
        string currentUserId, CreateTaRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(currentUserId);

        if (user is null)
            return Result.Failure(TaErrors.UserNotFound);

        var isTa = await userManager.IsInRoleAsync(user, DefaultRoles.TA);

        if (!isTa)
            return Result.Failure(TaErrors.UserNotTaRole);

        var alreadyHasProfile = await context.TeachingAssistants
            .AnyAsync(ta => ta.UserId == currentUserId, cancellationToken);

        if (alreadyHasProfile)
            return Result.Failure(TaErrors.UserAlreadyTa);

        var ta = request.Adapt<TeachingAssistant>();
        ta.UserId = currentUserId;
        ta.AvailableSlots = request.MaxSlots;

        await context.TeachingAssistants.AddAsync(ta, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(
        Guid id, UpdateTaRequest request, CancellationToken cancellationToken = default)
    {
        var ta = await context.FindAsync<TeachingAssistant>([id], cancellationToken);

        if (ta is null || ta.IsDeleted)
            return Result.Failure(TaErrors.TaNotFound);

        request.Adapt(ta);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var ta = await context.FindAsync<TeachingAssistant>([id], cancellationToken);

        if (ta is null || ta.IsDeleted)
            return Result.Failure(TaErrors.TaNotFound);

        ta.IsDeleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<IEnumerable<TAResponse>> MapWithRatingsAsync(
        IEnumerable<TeachingAssistant> tas, CancellationToken cancellationToken)
    {
        var response = new List<TAResponse>();

        foreach (var ta in tas)
        {
            var avgResult = await ratingService.GetAverageAsync(ta.Id, cancellationToken);
            var taResponse = mapper.Map<TAResponse>(ta) with
            {
                AverageRating = avgResult.IsSuccess ? avgResult.Value : 0
            };
            response.Add(taResponse);
        }

        return response;
    }
}