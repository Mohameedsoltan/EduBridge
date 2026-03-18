// using System.Security.Claims;
// using EduBridge.Abstractions;
// using EduBridge.Abstractions.Consts;
// using EduBridge.Contracts.TA;
// using EduBridge.Persistence;
// using EduBridge.Services.Interfaces;
// using Microsoft.EntityFrameworkCore;

// namespace EduBridge.Services;

// public class TaRequestService(
//     ApplicationDbContext context,
//     INotificationService notificationService,
//     IHttpContextAccessor httpContextAccessor) : ITaRequestService
// {
//     private string CurrentUserId => httpContextAccessor.HttpContext!.User
//         .FindFirstValue(ClaimTypes.NameIdentifier)!;

//     // Queries
//     public async Task<Result<IEnumerable<TaRequestResponse>>> GetIncomingRequestsAsync(
//         Guid taId, CancellationToken cancellationToken = default)
//     {
//         var ta = await context.TeachingAssistants
//             .FirstOrDefaultAsync(ta => ta.Id == taId, cancellationToken);

//         if (ta is null)
//             return Result.Failure<IEnumerable<TaRequestResponse>>(TAErrors.TANotFound);

//         if (ta.UserId != CurrentUserId)
//             return Result.Failure<IEnumerable<TaRequestResponse>>(TAErrors.NotAuthorized);

//         var requests = await context.TARequests
//             .Include(tr => tr.Team)
//             .Include(tr => tr.TA)
//             .ThenInclude(ta => ta.User)
//             .Where(tr => tr.TAId == taId)
//             .ToListAsync(cancellationToken);

//         var response = requests.Select(tr => new TaRequestResponse(
//             tr.Id,
//             tr.TeamId,
//             tr.Team.Name,
//             $"{tr.TA.User.FirstName} {tr.TA.User.LastName}",
//             tr.TA.Department,
//             tr.Status,
//             tr.Message,
//             tr.ResponseMessage,
//             tr.CreatedOn,
//             tr.RespondedAt
//         ));

//         return Result.Success(response);
//     }

//     public async Task<Result<IEnumerable<TaRequestResponse>>> GetTeamRequestsAsync(
//         Guid teamId, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

//         if (team is null)
//             return Result.Failure<IEnumerable<TaRequestResponse>>(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure<IEnumerable<TaRequestResponse>>(TeamErrors.NotTeamLeader);

//         var requests = await context.TARequests
//             .Include(tr => tr.Team)
//             .Include(tr => tr.TA)
//             .ThenInclude(ta => ta.User)
//             .Where(tr => tr.TeamId == teamId)
//             .ToListAsync(cancellationToken);

//         var response = requests.Select(tr => new TaRequestResponse(
//             tr.Id,
//             tr.TeamId,
//             tr.Team.Name,
//             $"{tr.TA.User.FirstName} {tr.TA.User.LastName}",
//             tr.TA.Department,
//             tr.Status,
//             tr.Message,
//             tr.ResponseMessage,
//             tr.CreatedOn,
//             tr.RespondedAt
//         ));

//         return Result.Success(response);
//     }

//     // Team leader operations
//     public async Task<Result> SendAsync(
//         Guid teamId, Guid taId, string? message, CancellationToken cancellationToken = default)
//     {
//         var team = await context.Teams
//             .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

//         if (team is null)
//             return Result.Failure(TeamErrors.TeamNotFound);

//         if (team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         var ta = await context.TeachingAssistants
//             .FirstOrDefaultAsync(ta => ta.Id == taId, cancellationToken);

//         if (ta is null)
//             return Result.Failure(TAErrors.TANotFound);

//         if (!ta.IsAvailable)
//             return Result.Failure(TAErrors.TANotAvailable);

//         var existingRequest = await context.TARequests
//             .FirstOrDefaultAsync(tr => tr.TeamId == teamId
//                 && tr.TAId == taId
//                 && tr.Status == RequestStatus.Pending, cancellationToken);

//         if (existingRequest is not null)
//             return Result.Failure(TARequestErrors.RequestAlreadyExists);

//         var request = new TARequest
//         {
//             TeamId = teamId,
//             TAId = taId,
//             Message = message,
//             Status = RequestStatus.Pending
//         };

//         await context.TARequests.AddAsync(request, cancellationToken);

//         // Update team status
//         team.Status = TeamStatus.TaPending;

//         await context.SaveChangesAsync(cancellationToken);

//         // Notify TA
//         await notificationService.SendAsync(
//             ta.UserId,
//             NotificationType.TARequestReceived,
//             $"Team {team.Name} has requested your supervision",
//             request.Id,
//             cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> CancelAsync(
//         Guid id, CancellationToken cancellationToken = default)
//     {
//         var request = await context.TARequests
//             .Include(tr => tr.Team)
//             .FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);

//         if (request is null)
//             return Result.Failure(TARequestErrors.RequestNotFound);

//         if (request.Team.LeaderId != CurrentUserId)
//             return Result.Failure(TeamErrors.NotTeamLeader);

//         if (request.Status != RequestStatus.Pending)
//             return Result.Failure(TARequestErrors.RequestNotPending);

//         request.Status = RequestStatus.Cancelled;

//         // Revert team status
//         request.Team.Status = TeamStatus.Full;

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     // TA operations
//     public async Task<Result> ApproveAsync(
//         Guid id, string? responseMessage, CancellationToken cancellationToken = default)
//     {
//         var request = await context.TARequests
//             .Include(tr => tr.Team)
//             .Include(tr => tr.TA)
//             .FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);

//         if (request is null)
//             return Result.Failure(TARequestErrors.RequestNotFound);

//         if (request.TA.UserId != CurrentUserId)
//             return Result.Failure(TAErrors.NotAuthorized);

//         if (request.Status != RequestStatus.Pending)
//             return Result.Failure(TARequestErrors.RequestNotPending);

//         request.Status = RequestStatus.Approved;
//         request.ResponseMessage = responseMessage;
//         request.RespondedByTAId = request.TAId;
//         request.RespondedAt = DateTime.UtcNow;

//         // Assign TA to team
//         request.Team.TaId = request.TAId;
//         request.Team.Status = TeamStatus.TaApproved;

//         // Decrease TA available slots
//         request.TA.AvailableSlots--;

//         await context.SaveChangesAsync(cancellationToken);

//         // Notify team leader
//         await notificationService.SendAsync(
//             request.Team.LeaderId,
//             NotificationType.TARequestAccepted,
//             $"Your request for TA supervision has been accepted",
//             request.Id,
//             cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> RejectAsync(
//         Guid id, string? responseMessage, CancellationToken cancellationToken = default)
//     {
//         var request = await context.TARequests
//             .Include(tr => tr.Team)
//             .Include(tr => tr.TA)
//             .FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);

//         if (request is null)
//             return Result.Failure(TARequestErrors.RequestNotFound);

//         if (request.TA.UserId != CurrentUserId)
//             return Result.Failure(TAErrors.NotAuthorized);

//         if (request.Status != RequestStatus.Pending)
//             return Result.Failure(TARequestErrors.RequestNotPending);

//         request.Status = RequestStatus.Rejected;
//         request.ResponseMessage = responseMessage;
//         request.RespondedByTAId = request.TAId;
//         request.RespondedAt = DateTime.UtcNow;

//         // Revert team status
//         request.Team.Status = TeamStatus.Full;

//         await context.SaveChangesAsync(cancellationToken);

//         // Notify team leader
//         await notificationService.SendAsync(
//             request.Team.LeaderId,
//             NotificationType.TARequestRejected,
//             $"Your request for TA supervision has been rejected",
//             request.Id,
//             cancellationToken);

//         return Result.Success();
//     }
// }