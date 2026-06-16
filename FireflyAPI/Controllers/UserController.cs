using System.Security.Claims;
using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FireflyAPI.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly ProjectService _projectService;

    public UserController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [Authorize]
    [HttpGet ("projects")]
    public async Task<IActionResult> UserProjects(CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        var result = await _projectService.GetUserProjects(cancellationToken);
        return Ok(result);
    }
}