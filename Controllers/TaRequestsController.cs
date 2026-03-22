using EduBridge.Abstractions;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TaRequestsController(
    ITaRequestService taRequestService,
    ILogger<TaRequestsController> logger) : ControllerBase
{
    [HttpGet("ta/{taId:guid}")]
    [Authorize(Roles = DefaultRoles.TA)]
    public async Task<IActionResult> GetIncomingRequestsAsync(
        [FromRoute] Guid taId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching incoming TA requests for TA {TaId}", taId);

        var result = await taRequestService.GetIncomingRequestsAsync(taId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("team/{teamId:guid}")]
    [Authorize(Roles = $"{DefaultRoles.Student}")]
    public async Task<IActionResult> GetTeamRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching TA requests for team {TeamId}", teamId);

        var result = await taRequestService.GetTeamRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("team/{teamId:guid}/ta/{taId:guid}")]
    [Authorize(Roles = DefaultRoles.Student)]
    public async Task<IActionResult> SendAsync(
        [FromRoute] Guid teamId,
        [FromRoute] Guid taId,
        [FromBody] string? message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Team {TeamId} sending TA request to TA {TaId}", teamId, taId);

        var result = await taRequestService.SendAsync(teamId, taId, message, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles = $"{DefaultRoles.Student}")]
    public async Task<IActionResult> CancelAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling TA request {RequestId}", id);

        var result = await taRequestService.CancelAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = DefaultRoles.TA)]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Approving TA request {RequestId}", id);

        var result = await taRequestService.ApproveAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = DefaultRoles.TA)]
    public async Task<IActionResult> RejectAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Rejecting TA request {RequestId}", id);

        var result = await taRequestService.RejectAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}