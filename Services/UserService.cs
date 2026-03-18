// using EduBridge.Abstractions;
// using EduBridge.Contracts.User;
// using EduBridge.Entities;
// using EduBridge.Errors;
// using EduBridge.Persistence;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;

// public class UserService(
//     UserManager<ApplicationUser> userManager,
//     ApplicationDbContext context) : IUserService
// {
//     // Queries
//     public async Task<Result<UserProfileResponse>> GetCurrentUserAsync(
//         string userId, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.Users
//             .Include(u => u.Skills)
//             .ThenInclude(us => us.Skill)
//             .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//         if (user is null)
//             return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

//         return Result.Success(new UserProfileResponse(
//             user.FirstName,
//             user.LastName,
//             user.Bio,
//             user.Major,
//             user.University,
//             user.ProfileImageUrl,
//             user.Skills.Select(s => s.Skill.Name)
//         ));
//     }

//     public async Task<Result<UserResponse>> GetUserByIdAsync(
//         string userId, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.FindByIdAsync(userId);

//         if (user is null)
//             return Result.Failure<UserResponse>(UserErrors.UserNotFound);

//         var roles = await userManager.GetRolesAsync(user);

//         return Result.Success(new UserResponse(
//             user.Id,
//             user.FirstName,
//             user.LastName,
//             user.Email!,
//             roles.FirstOrDefault() ?? string.Empty,
//             user.ProfileImageUrl,
//             user.CreatedAt
//         ));
//     }

//     public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(
//         CancellationToken cancellationToken = default)
//     {
//         var users = await userManager.Users
//             .ToListAsync(cancellationToken);

//         var response = new List<UserResponse>();

//         foreach (var user in users)
//         {
//             var roles = await userManager.GetRolesAsync(user);
//             response.Add(new UserResponse(
//                 user.Id,
//                 user.FirstName,
//                 user.LastName,
//                 user.Email!,
//                 roles.FirstOrDefault() ?? string.Empty,
//                 user.ProfileImageUrl,
//                 user.CreatedAt
//             ));
//         }

//         return Result.Success<IEnumerable<UserResponse>>(response);
//     }

//     // Profile operations
//     public async Task<Result<UserProfileResponse>> UpdateProfileAsync(
//         string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.FindByIdAsync(userId);

//         if (user is null)
//             return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

//         user.FirstName = request.FirstName ?? user.FirstName;
//         user.LastName = request.LastName ?? user.LastName;
//         user.Bio = request.Bio ?? user.Bio;
//         user.Major = request.Major ?? user.Major;
//         user.University = request.University ?? user.University;

//         var result = await userManager.UpdateAsync(user);

//         if (!result.Succeeded)
//         {
//             var error = result.Errors.First();
//             return Result.Failure<UserProfileResponse>(
//                 new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
//         }

//         return await GetCurrentUserAsync(userId, cancellationToken);
//     }

//     public async Task<Result> UploadProfileImageAsync(
//         string userId, IFormFile image, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.FindByIdAsync(userId);

//         if (user is null)
//             return Result.Failure(UserErrors.UserNotFound);

//         var imageUrl = await SaveImageAsync(image);

//         user.ProfileImageUrl = imageUrl;

//         await userManager.UpdateAsync(user);

//         return Result.Success();
//     }

//     public async Task<Result> ChangePasswordAsync(
//         string userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.FindByIdAsync(userId);

//         if (user is null)
//             return Result.Failure(UserErrors.UserNotFound);

//         if (request.NewPassword != request.ConfirmNewPassword)
//             return Result.Failure(UserErrors.PasswordMismatch);

//         var result = await userManager.ChangePasswordAsync(
//             user, request.CurrentPassword, request.NewPassword);

//         if (!result.Succeeded)
//         {
//             var error = result.Errors.First();
//             return Result.Failure(
//                 new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
//         }

//         return Result.Success();
//     }

//     // Skill operations
//     public async Task<Result<IEnumerable<SkillResponse>>> GetUserSkillsAsync(
//         string userId, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.Users
//             .Include(u => u.Skills)
//             .ThenInclude(us => us.Skill)
//             .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//         if (user is null)
//             return Result.Failure<IEnumerable<SkillResponse>>(UserErrors.UserNotFound);

//         var skills = user.Skills
//             .Select(us => new SkillResponse(us.Skill.Id, us.Skill.Name));

//         return Result.Success(skills);
//     }

//     public async Task<Result> AddSkillsAsync(
//         string userId, List<string> skillNames, CancellationToken cancellationToken = default)
//     {
//         var user = await userManager.Users
//             .Include(u => u.Skills)
//             .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//         if (user is null)
//             return Result.Failure(UserErrors.UserNotFound);

//         foreach (var skillName in skillNames)
//         {
//             var skill = await context.Skills
//                 .FirstOrDefaultAsync(s => s.Name == skillName, cancellationToken)
//                 ?? new Skill { Name = skillName };

//             if (skill.Id == Guid.Empty)
//                 await context.Skills.AddAsync(skill, cancellationToken);

//             if (!user.Skills.Any(us => us.SkillId == skill.Id))
//             {
//                 user.Skills.Add(new UserSkill
//                 {
//                     UserId = userId,
//                     SkillId = skill.Id
//                 });
//             }
//         }

//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> RemoveSkillAsync(
//         string userId, Guid skillId, CancellationToken cancellationToken = default)
//     {
//         var userSkill = await context.UserSkills
//             .FirstOrDefaultAsync(us => us.UserId == userId 
//                 && us.SkillId == skillId, cancellationToken);

//         if (userSkill is null)
//             return Result.Failure(UserErrors.SkillNotFound);

//         context.UserSkills.Remove(userSkill);
//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     public async Task<Result> ClearSkillsAsync(
//         string userId, CancellationToken cancellationToken = default)
//     {
//         var userSkills = await context.UserSkills
//             .Where(us => us.UserId == userId)
//             .ToListAsync(cancellationToken);

//         context.UserSkills.RemoveRange(userSkills);
//         await context.SaveChangesAsync(cancellationToken);

//         return Result.Success();
//     }

//     // Admin operations
//     public async Task<Result> AddAsync(
//         CreateUserRequest request, CancellationToken cancellationToken = default)
//     {
//         var existingUser = await userManager.FindByEmailAsync(request.Email);

//         if (existingUser is not null)
//             return Result.Failure(UserErrors.DuplicateEmail);

//         var user = new ApplicationUser
//         {
//             FirstName = request.FirstName,
//             LastName = request.LastName,
//             Email = request.Email,
//             UserName = request.Email,
//             Bio = request.Bio,
//             Major = request.Major,
//             University = request.University
//         };

//         var result = await userManager.CreateAsync(user, request.Password);

//         if (!result.Succeeded)
//         {
//             var error = result.Errors.First();
//             return Result.Failure(
//                 new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
//         }

//         await userManager.AddToRoleAsync(user, request.Role);

//         return Result.Success();
//     }

//     private async Task<string> SaveImageAsync(IFormFile image)
//     {
//         throw new NotImplementedException();
//     }
// }