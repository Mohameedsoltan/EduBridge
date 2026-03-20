using EduBridge.Contracts.TA;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Student)] // POST send
[Authorize(Roles = DefaultRoles.TA)] // PUT approve, PUT reject
[Authorize] // GET incoming, GET team requests
[Authorize(Roles = $"{DefaultRoles.Student},{DefaultRoles.TA}")] // PUT cancel
public class TaRequestsController(
    ITaRequestService taRequestService,
    ILogger<TaRequestsController> logger) : ControllerBase
{
    private readonly ITaRequestService _taRequestService = taRequestService;
    private readonly ILogger<TaRequestsController> _logger = logger;

    [HttpGet("ta/{taId:guid}")]
    public async Task<IActionResult> GetIncomingRequestsAsync(
        [FromRoute] Guid taId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching incoming TA requests for TA {TaId}", taId);

        var result = await _taRequestService.GetIncomingRequestsAsync(taId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetTeamRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching TA requests for team {TeamId}", teamId);

        var result = await _taRequestService.GetTeamRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("team/{teamId:guid}/ta/{taId:guid}")]
    public async Task<IActionResult> SendAsync(
        [FromRoute] Guid teamId,
        [FromRoute] Guid taId,
        [FromBody] string? message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Team {TeamId} sending TA request to TA {TaId}", teamId, taId);

        var result = await _taRequestService.SendAsync(teamId, taId, message, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling TA request {RequestId}", id);

        var result = await _taRequestService.CancelAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Approving TA request {RequestId}", id);

        var result = await _taRequestService.ApproveAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectAsync(
        [FromRoute] Guid id,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting TA request {RequestId}", id);

        var result = await _taRequestService.RejectAsync(id, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}