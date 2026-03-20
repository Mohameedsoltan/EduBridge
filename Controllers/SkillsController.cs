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
[Authorize(Roles = DefaultRoles.Admin)]
public class SkillsController(
    ISkillService skillService,
    ILogger<SkillsController> logger) : ControllerBase
{
    private readonly ISkillService _skillService = skillService;
    private readonly ILogger<SkillsController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all skills");

        var result = await _skillService.GetAllAsync(cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("get-or-create")]
    public async Task<IActionResult> GetOrCreateAsync(
        [FromBody] string skillName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting or creating skill with name {SkillName}", skillName);

        var result = await _skillService.GetOrCreateAsync(skillName, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateSkillRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating skill {SkillId}", id);

        var result = await _skillService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting skill {SkillId}", id);

        var result = await _skillService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}