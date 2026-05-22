using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FireflyAPI.Controllers;

[ApiController]
[Route("projects/{projectId}/resources")]
public class ResourceController : ControllerBase
{
    private readonly ResourceService _resourceService;
    
    public ResourceController(ResourceService resourceService)
    {
        _resourceService = resourceService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetResources([FromRoute] Guid projectId, CancellationToken ct)
    {
        var result = await _resourceService.GetProjectResources(projectId, ct);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateResource([FromRoute] Guid projectId, [FromBody] CreateResourceRequestDto request, CancellationToken ct)
    {
        var resourceId = await _resourceService.CreateResource(projectId, request, ct);

        return CreatedAtAction(nameof(CreateResource), new { projectId, resourceId });
    }
    
    [HttpPatch("{resourceId}")]
    public async Task<IActionResult> EditResource([FromRoute] Guid resourceId, [FromBody] EditResourceRequestDto request, CancellationToken ct)
    {
        await _resourceService.EditResource(resourceId, request, ct);

        return NoContent();
    }

    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> DeleteResource([FromRoute] Guid resourceId, CancellationToken ct)
    {
        await _resourceService.DeleteResource(resourceId, ct);

        return NoContent();
    }
}