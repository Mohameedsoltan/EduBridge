using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.TA;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBridge.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TasController(
    ITaService taService,
    ILogger<TasController> logger) : ControllerBase
{
    private readonly ILogger<TasController> _logger = logger;
    private readonly ITaService _taService = taService;


    [HttpGet("")]
    public async Task<IActionResult> GetAllTAsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all TAs");

        var result = await _taService.GetAllTAsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching TA with ID {TaId}", id);

        var result = await _taService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableTAsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all available TAs");

        var result = await _taService.GetAvailableTAsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("teams")]
    [Authorize(Roles = DefaultRoles.TA)]
    public async Task<IActionResult> GetSupervisedTeamsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching supervised teams for current TA");

        var result = await _taService.GetSupervisedTeamsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    // [Authorize(Roles = DefaultRoles.TA)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAsync(
        [FromQuery] string currenUserId,
        [FromBody] CreateTaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating TA profile for current user");

        var result = await _taService.CreateAsync(currenUserId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.TA)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateTaRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating TA with ID {TaId}", id);

        var result = await _taService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting TA with ID {TaId}", id);

        var result = await _taService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}