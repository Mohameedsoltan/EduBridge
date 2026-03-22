using System.Security.Claims;
using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Doctor;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class DoctorRequestsController(
    IDoctorRequestService doctorRequestService,
    ILogger<DoctorRequestsController> logger) : ControllerBase
{
    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Student)]
    public async Task<IActionResult> SendRequestAsync(
        [FromBody] SendDoctorRequestRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Team {TeamId} is sending a doctor request to doctor {DoctorId}",
            request.TeamId, request.DoctorId);

        var result = await doctorRequestService.CreateRequestAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{requestId:guid}/cancel")]
    public async Task<IActionResult> CancelRequestAsync(
        [FromRoute] Guid requestId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling doctor request {RequestId}", requestId);

        var result = await doctorRequestService.CancelRequestAsync(requestId, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPut("{requestId:guid}/approve")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> ApproveAsync(
    [FromRoute] Guid requestId,
    [FromBody] string? responseMessage,
    CancellationToken cancellationToken)
    {
        logger.LogInformation("Approving doctor request {RequestId}", requestId);

        var result = await doctorRequestService.ApproveAsync(requestId, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{requestId:guid}/reject")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> RejectAsync(
        [FromRoute] Guid requestId,
        [FromBody] string? responseMessage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Rejecting doctor request {RequestId}", requestId);

        var result = await doctorRequestService.RejectAsync(requestId, responseMessage, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("team/{teamId:guid}")]
    [Authorize(Roles = DefaultRoles.Student)]
    public async Task<IActionResult> GetTeamRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching doctor requests for team {TeamId}", teamId);

        var result = await doctorRequestService.GetTeamRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("doctor/{doctorId:guid}")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> GetDoctorRequestsAsync(
        [FromRoute] Guid doctorId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching requests received by doctor {DoctorId}", doctorId);

        var result = await doctorRequestService.GetDoctorRequestsAsync(doctorId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}