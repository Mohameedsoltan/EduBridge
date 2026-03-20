using EduBridge.Abstractions;
using EduBridge.Contracts.TA;
using EduBridge.Contracts.Team;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TasController(
    ITaService taService,
    ILogger<TasController> logger) : ControllerBase
{
    private readonly ITaService _taService = taService;
    private readonly ILogger<TasController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllTAsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all TAs");

        var result = await _taService.GetAllTAsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableTAsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all available TAs");

        var result = await _taService.GetAvailableTAsAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{userId}/teams")]
    public async Task<IActionResult> GetSupervisedTeamsAsync(
        [FromRoute] string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching supervised teams for TA with user ID {UserId}", userId);

        var result = await _taService.GetSupervisedTeamsAsync(userId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}