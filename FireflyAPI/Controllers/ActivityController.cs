using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FireflyAPI.Controllers;

[ApiController]
[Route("projects/{projectId}/activities")]
public class ActivityController : ControllerBase
{
    private readonly ActivityService _activityService;

    public ActivityController(ActivityService activityService)
    {
        _activityService = activityService;
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetActivities(CancellationToken ct)
    {
        var result = await _activityService.GetActivities(ct);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromRoute]Guid projectId,
        [FromBody]CreateActivityRequestDto activityRequestDto,CancellationToken ct)
    {
        var activityId = await _activityService.CreateActivity(projectId, activityRequestDto, ct);
        
        return CreatedAtAction(nameof(CreateActivity), new { projectId, activityId });;
    }

    [Authorize]
    [HttpPost("predecessors")]
    public async Task<IActionResult> AssignPredecessors([FromBody] AssignPredecessorsRequestDto assignPredecessorsRequestDto,
        CancellationToken ct = default)
    {
        await _activityService.AssignPredecessors(assignPredecessorsRequestDto, ct);
        return Ok();
    }
}