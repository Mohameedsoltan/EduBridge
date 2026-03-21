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
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all idea categories");

        var result = await ideaCategoryService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("get-or-create")]
    public async Task<IActionResult> GetOrCreateAsync(
        [FromBody] CreateIdeaCategoryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting or creating idea category with name {Name}", request.Name);

        var result = await ideaCategoryService.GetOrCreateAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIdeaCategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating idea category with ID {CategoryId}", id);

        var result = await ideaCategoryService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting idea category with ID {CategoryId}", id);

        var result = await ideaCategoryService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}