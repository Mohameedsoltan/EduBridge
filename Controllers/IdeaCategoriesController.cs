using EduBridge.Contracts.Idea;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[Authorize(Roles = DefaultRoles.Admin)]
public class IdeaCategoriesController(
    IIdeaCategoryService ideaCategoryService,
    ILogger<IdeaCategoriesController> logger) : ControllerBase
{
    private readonly IIdeaCategoryService _ideaCategoryService = ideaCategoryService;
    private readonly ILogger<IdeaCategoriesController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all idea categories");

        var result = await _ideaCategoryService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("get-or-create")]
    public async Task<IActionResult> GetOrCreateAsync(
        [FromBody] string name, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting or creating idea category with name {Name}", name);

        var result = await _ideaCategoryService.GetOrCreateAsync(name, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIdeaCategoryRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating idea category with ID {CategoryId}", id);

        var result = await _ideaCategoryService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting idea category with ID {CategoryId}", id);

        var result = await _ideaCategoryService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}