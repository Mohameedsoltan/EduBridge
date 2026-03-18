// using EduBridge.Abstractions;
// using EduBridge.Contracts.TA;
// using EduBridge.Contracts.Team;
// using EduBridge.Persistence;
// using EduBridge.Services.Interfaces;
// using Microsoft.EntityFrameworkCore;

// namespace EduBridge.Services;

// public class TaService(
//     ApplicationDbContext context,
//     IRatingService ratingService) : ITaService
// {
//     public async Task<Result<IEnumerable<TAResponse>>> GetAllTAsAsync(
//         CancellationToken cancellationToken = default)
//     {
//         var tas = await context.TeachingAssistants
//             .Include(ta => ta.User)
//             .ToListAsync(cancellationToken);

//         var response = new List<TAResponse>();

//         foreach (var ta in tas)
//         {
//             var avgRating = await ratingService.GetAverageAsync(ta.Id, cancellationToken);
//             response.Add(new TAResponse(
//                 ta.UserId,
//                 ta.User.FirstName,
//                 ta.User.LastName,
//                 ta.Department,
//                 ta.AcademicTitle,
//                 ta.OfficeLocation,
//                 ta.MaxSlots,
//                 ta.AvailableSlots,
//                 ta.IsAvailable,
//                 avgRating.IsSuccess ? avgRating.Value : 0
//             ));
//         }

//         return Result.Success<IEnumerable<TAResponse>>(response);
//     }

//     public async Task<Result<IEnumerable<TAResponse>>> GetAvailableTAsAsync(
//         CancellationToken cancellationToken = default)
//     {
//         var tas = await context.TeachingAssistants
//             .Include(ta => ta.User)
//             .Where(ta => ta.AvailableSlots > 0)
//             .ToListAsync(cancellationToken);

//         var response = new List<TAResponse>();

//         foreach (var ta in tas)
//         {
//             var avgRating = await ratingService.GetAverageAsync(ta.Id, cancellationToken);
//             response.Add(new TAResponse(
//                 ta.UserId,
//                 ta.User.FirstName,
//                 ta.User.LastName,
//                 ta.Department,
//                 ta.AcademicTitle,
//                 ta.OfficeLocation,
//                 ta.MaxSlots,
//                 ta.AvailableSlots,
//                 ta.IsAvailable,
//                 avgRating.IsSuccess ? avgRating.Value : 0
//             ));
//         }

//         return Result.Success<IEnumerable<TAResponse>>(response);
//     }

//     public async Task<Result<IEnumerable<TeamResponse>>> GetSupervisedTeamsAsync(
//         string userId, CancellationToken cancellationToken = default)
//     {
//         var ta = await context.TeachingAssistants
//             .FirstOrDefaultAsync(ta => ta.UserId == userId, cancellationToken);

//         if (ta is null)
//             return Result.Failure<IEnumerable<TeamResponse>>(TAErrors.TANotFound);

//         var teams = await context.Teams
//             .Include(t => t.Members)
//             .Where(t => t.TaId == ta.Id)
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

//         return Result.Success<IEnumerable<TeamResponse>>(response);
//     }
// }