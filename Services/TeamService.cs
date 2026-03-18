// using System.Security.Claims;
// using EduBridge.Abstractions;
// using EduBridge.Abstractions.Consts;
// using EduBridge.Contracts.Team;
// using EduBridge.Entities;
// using EduBridge.Persistence;
// using EduBridge.Services.Interfaces;
// using Microsoft.EntityFrameworkCore;

// namespace EduBridge.Services;

// public class TeamService(
//     ApplicationDbContext context,
//     INotificationService notificationService,
//     IHttpContextAccessor httpContextAccessor) : ITeamService
// {
//     private string CurrentUserId => httpContextAccessor.HttpContext!.User
//         .FindFirstValue(ClaimTypes.NameIdentifier)!;

//     // Queries
//     public async Task<Result<TeamResponse>> GetByIdAsync(
//         Guid id, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .Include(t => t.Members)
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure<TeamResponse>(TeamErrors.TeamNotFound);

//         return Result.Success(new TeamResponse(
//             team.Id,
//             team.Name,
//             team.Description,
//             team.LeaderId,
//             team.MaxMembers,
//             team.Members.Count,
//             team.Status,
//             team.CreatedOn
//         ));
//     }

//     public async Task<Result<IEnumerable<TeamResponse>>> GetAllAsync(
//         CancellationToken cancellationToken = default)
//     {
//         var teams = await context.Teams
//             .Include(t => t.Members)
//             .ToListAsync(cancellationToken);

//         var response = teams.Select(t => new TeamResponse(
//             t.Id,
//             t.Name,
//             t.Description,
//             t.LeaderId,
//             t.MaxMembers,
//             t.Members.Count,
//             t.Status,
//             t.CreatedOn
//         ));

//         return Result.Success(response);
//     }

//     // Team management
//     public async Task<Result<TeamResponse>> CreateAsync(
//         CreateTeamRequest request, CancellationToken cancellationToken = default)
//     {
//         var team = new Team
//         {
//             Name = request.Name,
//             Description = request.Description,
//             LeaderId = CurrentUserId,
//             Status = TeamStatus.Open
//         };

//         await context.Teams.AddAsync(team, cancellationToken);

//         // Add leader as first member
//         await context.TeamMembers.AddAsync(new TeamMember
//         {
//             TeamId = team.Id,
//             UserId = CurrentUserId,
//             Role = MemberRole.Leader,
//             JoinedAt = DateTime.UtcNow
//         }, cancellationToken);

//         await context.SaveChangesAsync(cancellationToken);

//         return await GetByIdAsync(team.Id, cancellationToken);
//     }

//     public async Task<Result<TeamResponse>> UpdateAsync(
//         Guid id, UpdateTeamRequest request, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure<TeamResponse>(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure<TeamResponse>(TeamErrors.NotTeamLeader);

//         team.Name = request.Name ?? team.Name;
//         team.Description = request.Description ?? team.Description;

//         await context.SaveChangesAsync(cancellationToken);

//         return await GetByIdAsync(id, cancellationToken);
//     }

//     public async Task<Result> DeleteAsync(
//         Guid id, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         context.Teams.Remove(team);
//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     // Members management
//     public async Task<Result> AddMemberAsync(
//         Guid id, string userId, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .Include(t => t.Members)
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         if (team.Members.Any(m => m.UserId == userId))
//             return Result.Failure(TeamErrors.AlreadyMember);

//         if (team.Members.Count >= team.MaxMembers)
//             return Result.Failure(TeamErrors.TeamFull);

//         await context.TeamMembers.AddAsync(new TeamMember
//         {
//             TeamId = id,
//             UserId = userId,
//             Role = MemberRole.Member,
//             JoinedAt = DateTime.UtcNow
//         }, cancellationToken);

//         // Update team status
//         if (team.Members.Count + 1 >= team.MaxMembers)
//             team.Status = TeamStatus.Full;
//         else
//             team.Status = TeamStatus.Partial;

//         await context.SaveChangesAsync(cancellationToken);

//         // Notify new member
//         await notificationService.SendAsync(
//             userId,
//             NotificationType.TeamMemberJoined,
//             $"You have been added to team {team.Name}",
//             team.Id,
//             cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> RemoveMemberAsync(
//         Guid id, string userId, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .Include(t => t.Members)
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         var member = team.Members.FirstOrDefault(m => m.UserId == userId);

//         if (member is null)
//             return Result.Failure(TeamErrors.MemberNotFound);

//         context.TeamMembers.Remove(member);

//         // Update team status
//         if (team.Members.Count - 1 < team.MaxMembers)
//             team.Status = TeamStatus.Partial;

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> LeaveAsync(
//         Guid id, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .Include(t => t.Members)
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId == CurrentUserId)
//             return Result.Failure(TeamErrors.LeaderCannotLeave);

//         var member = team.Members.FirstOrDefault(m => m.UserId == CurrentUserId);

//         if (member is null)
//             return Result.Failure(TeamErrors.MemberNotFound);

//         context.TeamMembers.Remove(member);

//         // Update team status
//         if (team.Members.Count - 1 < team.MaxMembers)
//             team.Status = TeamStatus.Partial;

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     // Status
//     public async Task<Result> ChangeStatusAsync(
//         Guid id, TeamStatus status, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         team.Status = status;

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     // Idea
//     public async Task<Result> AssignIdeaAsync(
//         Guid teamId, Guid ideaId, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         var idea = await context.Ideas
//             .FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);

//         if (idea is null)
//             return Result.Failure(TeamErrors.IdeaNotFound);

//         team.IdeaId = ideaId;
//         team.Status = TeamStatus.IdeaSelection;

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }
// }