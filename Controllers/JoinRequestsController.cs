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
    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetIncomingRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching incoming join requests for team {TeamId}", teamId);

        var result = await joinRequestService.GetIncomingRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("user/{studentId}")]
    public async Task<IActionResult> GetUserRequestsAsync(
        [FromRoute] string studentId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching join requests for student {StudentId}", studentId);

        var result = await joinRequestService.GetUserRequestsAsync(studentId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("team/{teamId:guid}")]
    public async Task<IActionResult> SendAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sending join request to team {TeamId}", teamId);

        var result = await joinRequestService.SendAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling join request {RequestId}", id);

        var result = await joinRequestService.CancelAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Approving join request {RequestId}", id);

        var result = await joinRequestService.ApproveAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Rejecting join request {RequestId}", id);

        var result = await joinRequestService.RejectAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}