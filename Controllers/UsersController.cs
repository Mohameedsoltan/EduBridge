using EduBridge.Contracts.User;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Admin)] 
public class UsersController(
    IUserService userService,
    ILogger<UsersController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<UsersController> _logger = logger;

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching profile for current user {UserId}", CurrentUserId);

        var result = await _userService.GetCurrentUserAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserByIdAsync(
        [FromRoute] string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user {UserId}", userId);

        var result = await _userService.GetUserByIdAsync(userId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all users");

        var result = await _userService.GetAllUsersAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> AddAsync(
        [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with email {Email}", request.Email);

        var result = await _userService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfileAsync(
        [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating profile for current user {UserId}", CurrentUserId);

        var result = await _userService.UpdateProfileAsync(CurrentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("me/profile-image")]
    public async Task<IActionResult> UploadProfileImageAsync(
        [FromForm] IFormFile image, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading profile image for current user {UserId}", CurrentUserId);

        var result = await _userService.UploadProfileImageAsync(CurrentUserId, image, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing password for current user {UserId}", CurrentUserId);

        var result = await _userService.ChangePasswordAsync(CurrentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("me/skills")]
    public async Task<IActionResult> GetUserSkillsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching skills for current user {UserId}", CurrentUserId);

        var result = await _userService.GetUserSkillsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("me/skills")]
    public async Task<IActionResult> AddSkillsAsync(
        [FromBody] List<string> skillNames, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding skills for current user {UserId}", CurrentUserId);

        var result = await _userService.AddSkillsAsync(CurrentUserId, skillNames, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("me/skills/{skillId:guid}")]
    public async Task<IActionResult> RemoveSkillAsync(
        [FromRoute] Guid skillId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing skill {SkillId} from current user {UserId}", skillId, CurrentUserId);

        var result = await _userService.RemoveSkillAsync(CurrentUserId, skillId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("me/skills")]
    public async Task<IActionResult> ClearSkillsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing all skills for current user {UserId}", CurrentUserId);

        var result = await _userService.ClearSkillsAsync(CurrentUserId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}