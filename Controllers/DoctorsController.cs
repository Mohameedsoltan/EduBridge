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
    private readonly IDoctorService _doctorService = doctorService;
    private readonly ILogger<DoctorsController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all doctors");

        var result = await _doctorService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching doctor with ID {DoctorId}", id);

        var result = await _doctorService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableDoctorsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all available doctors");

        var result = await _doctorService.GetAvailableDoctorsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> CreateAsync(
        string currentUserId, [FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating doctor profile for current user");

        var result = await _doctorService.CreateAsync(currentUserId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating doctor with ID {DoctorId}", id);

        var result = await _doctorService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting doctor with ID {DoctorId}", id);

        var result = await _doctorService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}