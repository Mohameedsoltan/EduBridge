using EduBridge.Abstractions;
using EduBridge.Contracts.Doctor;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EduBridge.Abstractions.Consts;

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

    [HttpPut("{requestId:guid}/respond")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> RespondToRequestAsync(
        [FromRoute] Guid requestId,
        [FromBody] RespondDoctorRequestDto request,
        CancellationToken cancellationToken)
    {
        var doctorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        logger.LogInformation(
            "Doctor {DoctorUserId} is responding to request {RequestId}",
            doctorUserId, requestId);

        var result = await doctorRequestService.RespondToRequestAsync(
            requestId, doctorUserId!, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetTeamRequestsAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching doctor requests for team {TeamId}", teamId);

        var result = await doctorRequestService.GetTeamRequestsAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetDoctorRequestsAsync(
        [FromRoute] Guid doctorId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching requests received by doctor {DoctorId}", doctorId);

        var result = await doctorRequestService.GetDoctorRequestsAsync(doctorId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}