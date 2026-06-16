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
    public async Task<IActionResult> GetActivities([FromRoute]Guid projectId, CancellationToken ct)
    {
        var result = await _activityService.GetActivities(projectId, ct);


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

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetActivityById(Guid id, CancellationToken ct)
    {
        try
        {
           var activityDetails = await _activityService.GetActivityDetails(id, ct);
            return Ok(activityDetails);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize]
    [HttpPatch("{activityId}")]
    public async Task<IActionResult> EditActivity([FromRoute] Guid activityId, [FromBody] EditActivityRequestDto request,
        CancellationToken ct)
    {
        await _activityService.Edit(activityId, request, ct);

        return Ok();
    }
    
    [Authorize]
    [HttpDelete("{activityId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid activityId, CancellationToken ct = default)
    {
        await _activityService.DeleteActivity(activityId, ct);
        return NoContent();
    }
}