using EduBridge.Contracts.Rating;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Student)] 
public class RatingsController(
    IRatingService ratingService,
    ILogger<RatingsController> logger) : ControllerBase
{
    private readonly IRatingService _ratingService = ratingService;
    private readonly ILogger<RatingsController> _logger = logger;

    [HttpGet("ta/{taId:guid}")]
    public async Task<IActionResult> GetByTaAsync(
        [FromRoute] Guid taId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching ratings for TA {TaId}", taId);

        var result = await _ratingService.GetByTaAsync(taId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<IActionResult> GetByTeamAsync(
        [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching rating for team {TeamId}", teamId);

        var result = await _ratingService.GetByTeamAsync(teamId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("ta/{taId:guid}/average")]
    public async Task<IActionResult> GetAverageAsync(
        [FromRoute] Guid taId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching average rating for TA {TaId}", taId);

        var result = await _ratingService.GetAverageAsync(taId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("team/{teamId:guid}")]
    public async Task<IActionResult> SubmitAsync(
        [FromRoute] Guid teamId,
        [FromBody] SubmitRatingRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting rating for team {TeamId}", teamId);

        var result = await _ratingService.SubmitAsync(teamId, request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}