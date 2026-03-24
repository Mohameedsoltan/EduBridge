using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Team;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Student)]
public class TeamsController(
    ITeamService teamService,
    ILogger<TeamsController> logger) : ControllerBase
{
    private readonly ILogger<TeamsController> _logger = logger;
    private readonly ITeamService _teamService = teamService;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all teams");

        var result = await _teamService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching team {TeamId}", id);

        var result = await _teamService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new team");

        var result = await _teamService.CreateAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateTeamRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating team {TeamId}", id);

        var result = await _teamService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting team {TeamId}", id);

        var result = await _teamService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("{id:guid}/members/{userId}")]
    public async Task<IActionResult> AddMemberAsync(
        [FromRoute] Guid id,
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding user {UserId} to team {TeamId}", userId, id);

        var result = await _teamService.AddMemberAsync(id, userId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{id:guid}/members/{userId}")]
    public async Task<IActionResult> RemoveMemberAsync(
        [FromRoute] Guid id,
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing user {UserId} from team {TeamId}", userId, id);

        var result = await _teamService.RemoveMemberAsync(id, userId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{id:guid}/members/me")]
    public async Task<IActionResult> LeaveAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Current user leaving team {TeamId}", id);

        var result = await _teamService.LeaveAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatusAsync(
        [FromRoute] Guid id,
        [FromBody] ChangeTeamStatusRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing status of team {TeamId} to {Status}", id, request.Status);

        var result = await _teamService.ChangeStatusAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/members/{userId}/assign-leader")]
    public async Task<IActionResult> AssignLeaderAsync(
        [FromRoute] Guid id,
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning user {UserId} as leader of team {TeamId}", userId, id);

        var result = await _teamService.AssignLeaderAsync(id, userId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}