using EduBridge.Contracts.Team;
using EduBridge.Entities;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Student)] 
public class TeamsController(
    ITeamService teamService,
    ILogger<TeamsController> logger) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;
    private readonly ILogger<TeamsController> _logger = logger;

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
        [FromBody] TeamStatus status,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing status of team {TeamId} to {Status}", id, status);

        var result = await _teamService.ChangeStatusAsync(id, status, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/idea/{ideaId:guid}")]
    public async Task<IActionResult> AssignIdeaAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid ideaId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning idea {IdeaId} to team {TeamId}", ideaId, id);

        var result = await _teamService.AssignIdeaAsync(id, ideaId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}