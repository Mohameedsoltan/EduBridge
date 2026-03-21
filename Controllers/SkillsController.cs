using EduBridge.Contracts.Skills;
using EduBridge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduBridge.Abstractions.Consts;
using EduBridge.Abstractions;
namespace EduBridge.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class SkillsController(
    ISkillService skillService,
    ILogger<SkillsController> logger) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching all skills");

        var result = await skillService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("get-or-create")]
    public async Task<IActionResult> GetOrCreateAsync(
        [FromBody] CreateSkillRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting or creating skill with name {SkillName}", request.Name);

        var result = await skillService.GetOrCreateAsync(request.Name, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateSkillRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating skill {SkillId}", id);

        var result = await skillService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting skill {SkillId}", id);

        var result = await skillService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}