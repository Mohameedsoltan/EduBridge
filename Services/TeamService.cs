using System.Security.Claims;
using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Team;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class TeamService(
    ApplicationDbContext context,
    INotificationService notificationService,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : ITeamService
{
    private string CurrentUserId => httpContextAccessor.HttpContext!.User
        .FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<Result<TeamResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var team = await context.Teams
            .AsNoTracking()
            .Include(t => t.Members)
            .Include(t => t.Leader)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
            return Result.Failure<TeamResponse>(TeamErrors.TeamNotFound);

        return Result.Success(mapper.Map<TeamResponse>(team));
    }

    public async Task<Result<IEnumerable<TeamResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var teams = await context.Teams
            .AsNoTracking()
            .Include(t => t.Members)
            .Include(t => t.Leader)
            .ToListAsync(cancellationToken);

        return Result.Success(mapper.Map<IEnumerable<TeamResponse>>(teams));
    }

    public async Task<Result<TeamResponse>> CreateAsync(
        CreateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var team = new Team
        {
            Name = request.Name,
            Description = request.Description,
            LeaderId = CurrentUserId,
            MaxMembers = TeamSettings.MaxMembers, 
            Status = TeamStatus.Open
        };


        await context.Teams.AddAsync(team, cancellationToken);

        await context.TeamMembers.AddAsync(new TeamMember
        {
            TeamId = team.Id,
            UserId = CurrentUserId,
            Role = MemberRole.Leader,
            JoinedAt = DateTime.UtcNow
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(team.Id, cancellationToken);
    }

    public async Task<Result<TeamResponse>> UpdateAsync(
        Guid id, UpdateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var team = await context.Teams
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
            return Result.Failure<TeamResponse>(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure<TeamResponse>(TeamErrors.NotTeamLeader);

        if (request.Name is not null) team.Name = request.Name;
        if (request.Description is not null) team.Description = request.Description;

        await context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var team = await context.FindAsync<Team>([id], cancellationToken);

        if (team is null || team.IsDeleted)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure(TeamErrors.NotTeamLeader);

        team.IsDeleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AddMemberAsync(
        Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var team = await context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure(TeamErrors.NotTeamLeader);

        if (team.Members.Any(m => m.UserId == userId))
            return Result.Failure(TeamErrors.AlreadyMember);

        if (team.Members.Count >= team.MaxMembers)
            return Result.Failure(TeamErrors.TeamFull);

        await context.TeamMembers.AddAsync(new TeamMember
        {
            TeamId = id,
            UserId = userId,
            Role = MemberRole.Member,
            JoinedAt = DateTime.UtcNow
        }, cancellationToken);

        team.Status = team.Members.Count + 1 >= team.MaxMembers
            ? TeamStatus.Full
            : TeamStatus.Partial;

        await context.SaveChangesAsync(cancellationToken);

        await notificationService.SendAsync(
            userId,
            NotificationType.TeamMemberJoined,
            $"You have been added to team {team.Name}",
            team.Id,
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveMemberAsync(
        Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var team = await context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure(TeamErrors.NotTeamLeader);

        var member = team.Members.FirstOrDefault(m => m.UserId == userId);

        if (member is null)
            return Result.Failure(TeamErrors.MemberNotFound);

        context.TeamMembers.Remove(member);

        if (team.Members.Count - 1 < team.MaxMembers)
            team.Status = TeamStatus.Partial;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> LeaveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var team = await context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId == CurrentUserId)
            return Result.Failure(TeamErrors.LeaderCannotLeave);

        var member = team.Members.FirstOrDefault(m => m.UserId == CurrentUserId);

        if (member is null)
            return Result.Failure(TeamErrors.MemberNotFound);

        context.TeamMembers.Remove(member);

        if (team.Members.Count - 1 < team.MaxMembers)
            team.Status = TeamStatus.Partial;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangeStatusAsync(
        Guid id, TeamStatus status, CancellationToken cancellationToken = default)
    {
        var team = await context.FindAsync<Team>([id], cancellationToken);

        if (team is null || team.IsDeleted)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure(TeamErrors.NotTeamLeader);

        team.Status = status;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AssignIdeaAsync(
        Guid teamId, Guid ideaId, CancellationToken cancellationToken = default)
    {
        var team = await context.FindAsync<Team>([teamId], cancellationToken);

        if (team is null || team.IsDeleted)
            return Result.Failure(TeamErrors.TeamNotFound);

        if (team.LeaderId != CurrentUserId)
            return Result.Failure(TeamErrors.NotTeamLeader);

        var ideaExists = await context.Ideas
            .AnyAsync(i => i.Id == ideaId, cancellationToken);

        if (!ideaExists)
            return Result.Failure(TeamErrors.IdeaNotFound);

        team.IdeaId = ideaId;
        team.Status = TeamStatus.IdeaSelection;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}