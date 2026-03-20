using EduBridge.Contracts.Role;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Admin)] 
public class RolesController(
    IRoleService roleService,
    ILogger<RolesController> logger) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;
    private readonly ILogger<RolesController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all roles");

        var result = await _roleService.GetAllRolesAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(
        [FromRoute] string roleId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching role {RoleId}", roleId);

        var result = await _roleService.GetRoleByIdAsync(roleId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateRoleAsync(
        [FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating role {RoleName}", request.Name);

        var result = await _roleService.CreateRoleAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRoleAsync(
        [FromRoute] string roleId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating role {RoleId}", roleId);

        var result = await _roleService.UpdateRoleAsync(roleId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteRoleAsync(
        [FromRoute] string roleId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting role {RoleId}", roleId);

        var result = await _roleService.DeleteRoleAsync(roleId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRolesAsync(
        [FromRoute] string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching roles for user {UserId}", userId);

        var result = await _roleService.GetUserRolesAsync(userId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoleToUserAsync(
        [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning role {RoleName} to user {UserId}", request.RoleName, request.UserId);

        var result = await _roleService.AssignRoleToUserAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRoleFromUserAsync(
        [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing role {RoleName} from user {UserId}", request.RoleName, request.UserId);

        var result = await _roleService.RemoveRoleFromUserAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}