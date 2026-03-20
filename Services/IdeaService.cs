using EduBridge.Abstractions;
using EduBridge.Contracts.Idea;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class IdeaService(
    ApplicationDbContext context,
    IIdeaTagService tagService) : IIdeaService
{
    public async Task<Result<IdeaResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var idea = await context.Ideas
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (idea is null)
            return Result.Failure<IdeaResponse>(IdeaErrors.IdeaNotFound);

        return Result.Success(MapToResponse(idea));
    }

    public async Task<Result<IEnumerable<IdeaResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var ideas = await context.Ideas
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Team)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<IdeaResponse>>(ideas.Select(MapToResponse));
    }

    public async Task<Result<IEnumerable<IdeaResponse>>> GetByCategoryAsync(
        Guid categoryId, CancellationToken cancellationToken = default)
    {
        var ideas = await context.Ideas
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Team)
            .Where(i => i.CategoryId == categoryId)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<IdeaResponse>>(ideas.Select(MapToResponse));
    }

    public async Task<Result<IEnumerable<IdeaResponse>>> GetByTagAsync(
        Guid tagId, CancellationToken cancellationToken = default)
    {
        var ideas = await context.Ideas
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Team)
            .Where(i => i.Tags.Any(t => t.Id == tagId))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<IdeaResponse>>(ideas.Select(MapToResponse));
    }

    public async Task<Result<IdeaResponse>> CreateAsync(
        CreateIdeaRequest request, CancellationToken cancellationToken = default)
    {
        var teamHasIdea = await context.Ideas
            .AnyAsync(i => i.TeamId == request.TeamId, cancellationToken);

        if (teamHasIdea)
            return Result.Failure<IdeaResponse>(IdeaErrors.TeamAlreadyHasIdea);

        // Resolve tags — get or create each one under the given category
        var tagIds = new List<Guid>();
        foreach (var tagName in request.Tags)
        {
            var tagResult = await tagService.GetOrCreateAsync(tagName, request.CategoryId, cancellationToken);
            if (tagResult.IsFailure)
                return Result.Failure<IdeaResponse>(tagResult.Error);
            tagIds.Add(tagResult.Value);
        }

        var tags = await context.IdeaTags
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        var idea = new Idea
        {
            Title = request.Title,
            Description = request.Description,
            RepositoryUrl = request.RepositoryUrl,
            TeamId = request.TeamId,
            CategoryId = request.CategoryId,
            Tags = tags
        };

        await context.Ideas.AddAsync(idea, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.Entry(idea).Reference(i => i.Category).LoadAsync(cancellationToken);
        await context.Entry(idea).Reference(i => i.Team).LoadAsync(cancellationToken);

        return Result.Success(MapToResponse(idea));
    }

    public async Task<Result<IdeaResponse>> UpdateAsync(
        Guid id, UpdateIdeaRequest request, CancellationToken cancellationToken = default)
    {
        var idea = await context.Ideas
            .Include(i => i.Category)
            .Include(i => i.Tags)
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (idea is null)
            return Result.Failure<IdeaResponse>(IdeaErrors.IdeaNotFound);

        if (request.Title is not null)
            idea.Title = request.Title;

        if (request.Description is not null)
            idea.Description = request.Description;

        if (request.RepositoryUrl is not null)
            idea.RepositoryUrl = request.RepositoryUrl;

        if (request.CategoryId is not null)
            idea.CategoryId = request.CategoryId.Value;

        if (request.Tags is not null)
        {
            var categoryId = request.CategoryId ?? idea.CategoryId;
            var tagIds = new List<Guid>();

            foreach (var tagName in request.Tags)
            {
                var tagResult = await tagService.GetOrCreateAsync(tagName, categoryId, cancellationToken);
                if (tagResult.IsFailure)
                    return Result.Failure<IdeaResponse>(tagResult.Error);
                tagIds.Add(tagResult.Value);
            }

            idea.Tags = await context.IdeaTags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync(cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(idea));
    }

    public async Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var idea = await context.FindAsync<Idea>([id], cancellationToken);

        if (idea is null || idea.IsDeleted)
            return Result.Failure(IdeaErrors.IdeaNotFound);

        idea.IsDeleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static IdeaResponse MapToResponse(Idea idea) => new(
        idea.Id,
        idea.Title,
        idea.Description,
        idea.RepositoryUrl,
        idea.Category.Name,
        idea.Tags.Select(t => t.Name),
        idea.Team.Name,
        idea.CreatedAt
    );
}