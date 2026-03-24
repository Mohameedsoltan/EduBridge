using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Idea;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class IdeasController(
    IIdeaService ideaService,
    ILogger<IdeasController> logger) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all ideas");

        var result = await ideaService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching idea {IdeaId}", id);

        var result = await ideaService.GetByIdAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching ideas for category {CategoryId}", categoryId);

        var result = await ideaService.GetByCategoryAsync(categoryId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("tag/{tagId:guid}")]
    public async Task<IActionResult> GetByTagAsync([FromRoute] Guid tagId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching ideas for tag {TagId}", tagId);

        var result = await ideaService.GetByTagAsync(tagId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateIdeaRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating idea for team {TeamId}", request.TeamId);

        var result = await ideaService.CreateAsync(request.TeamId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Student)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIdeaRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating idea {IdeaId}", id);

        var result = await ideaService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Student)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting idea {IdeaId}", id);

        var result = await ideaService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}