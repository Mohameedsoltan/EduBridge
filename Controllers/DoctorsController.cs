using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Doctor;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBridge.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class DoctorsController(
    IDoctorService doctorService,
    ILogger<DoctorsController> logger) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all doctors");

        var result = await doctorService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching doctor with ID {DoctorId}", id);

        var result = await doctorService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableDoctorsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all available doctors");

        var result = await doctorService.GetAvailableDoctorsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("me/teams")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> GetSupervisedTeamsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching supervised teams for current doctor");

        var result = await doctorService.GetSupervisedTeamsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAsync(
        [FromQuery] string currentUserId, [FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating doctor profile for current user");

        var result = await doctorService.CreateAsync(currentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating doctor with ID {DoctorId}", id);

        var result = await doctorService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting doctor with ID {DoctorId}", id);

        var result = await doctorService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}