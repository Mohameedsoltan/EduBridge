using EduBridge.Contracts.Skills;
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
[Authorize(Roles = DefaultRoles.Student)]
public class JoinRequestsController(
    IJoinRequestService joinRequestService,
    ILogger<JoinRequestsController> logger) : ControllerBase
{
    private readonly IJoinRequestService _joinRequestService = joinRequestService;
    private readonly ILogger<JoinRequestsController> _logger = logger;

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetIncomingRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching incoming join requests for team {TeamId}", teamId);

        var result = await _joinRequestService.GetIncomingRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("user/{studentId}")]
    public async Task<IActionResult> GetUserRequestsAsync(
        [FromRoute] string studentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching join requests for student {StudentId}", studentId);

        var result = await _joinRequestService.GetUserRequestsAsync(studentId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("team/{teamId:guid}")]
    public async Task<IActionResult> SendAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending join request to team {TeamId}", teamId);

        var result = await _joinRequestService.SendAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling join request {RequestId}", id);

        var result = await _joinRequestService.CancelAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving join request {RequestId}", id);

        var result = await _joinRequestService.ApproveAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting join request {RequestId}", id);

        var result = await _joinRequestService.RejectAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}