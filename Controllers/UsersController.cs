using EduBridge.Contracts.User;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController(
    IUserService userService,
    ILogger<UsersController> logger) : ControllerBase
{
    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching profile for current user {UserId}", CurrentUserId);

        var result = await userService.GetCurrentUserAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetUserByIdAsync(
        [FromRoute] string userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching user {UserId}", userId);

        var result = await userService.GetUserByIdAsync(userId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all users");

        var result = await userService.GetAllUsersAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> AddAsync(
        [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user with email {Email}", request.Email);

        var result = await userService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfileAsync(
        [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating profile for current user {UserId}", CurrentUserId);

        var result = await userService.UpdateProfileAsync(CurrentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("me/profile-image")]
    public async Task<IActionResult> UploadProfileImageAsync(
        [FromForm] IFormFile image, CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading profile image for current user {UserId}", CurrentUserId);

        var result = await userService.UploadProfileImageAsync(CurrentUserId, image, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Changing password for current user {UserId}", CurrentUserId);

        var result = await userService.ChangePasswordAsync(CurrentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("me/skills")]
    public async Task<IActionResult> GetUserSkillsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching skills for current user {UserId}", CurrentUserId);

        var result = await userService.GetUserSkillsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("me/skills")]
    public async Task<IActionResult> AddSkillsAsync(
        [FromBody] List<string> skillNames, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding skills for current user {UserId}", CurrentUserId);

        var result = await userService.AddSkillsAsync(CurrentUserId, skillNames, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("me/skills/{skillId:guid}")]
    public async Task<IActionResult> RemoveSkillAsync(
        [FromRoute] Guid skillId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing skill {SkillId} from current user {UserId}", skillId, CurrentUserId);

        var result = await userService.RemoveSkillAsync(CurrentUserId, skillId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("me/skills")]
    public async Task<IActionResult> ClearSkillsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Clearing all skills for current user {UserId}", CurrentUserId);

        var result = await userService.ClearSkillsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}