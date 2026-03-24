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
[Authorize(Roles = DefaultRoles.Admin)]
public class IdeaTagsController(
    IIdeaTagService ideaTagService,
    ILogger<IdeaTagsController> logger) : ControllerBase
{
    private readonly IIdeaTagService _ideaTagService = ideaTagService;
    private readonly ILogger<IdeaTagsController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all idea tags");

        var result = await _ideaTagService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching idea tags for category {CategoryId}", categoryId);

        var result = await _ideaTagService.GetByCategoryAsync(categoryId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("get-or-create")]
    public async Task<IActionResult> GetOrCreateAsync(
        CreateIdeaTagRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting or creating idea tag with name {Name} under category {CategoryId}",
            request.Name, request.CategoryId);

        var result = await _ideaTagService.GetOrCreateAsync(request.Name, request.CategoryId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIdeaTagRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating idea tag with ID {TagId}", id);

        var result = await _ideaTagService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting idea tag with ID {TagId}", id);

        var result = await _ideaTagService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}