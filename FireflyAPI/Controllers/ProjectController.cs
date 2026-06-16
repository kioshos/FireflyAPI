using System.Security.Claims;
using FireflyAPI.Application.Dtos;
using FireflyAPI.Application.Interfaces;
using FireflyAPI.Application.Services;
using FireflyAPI.Domain.Entities;
using FireflyAPI.Dtos;
using FireflyAPI.Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FireflyAPI.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _projectService;
    
    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProjects(CancellationToken ct)
    {
        var result = await _projectService.GetProjects(ct);

        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken ct)
    {
        var result = await _projectService.GetProject(id, ct);

        return Ok(result);
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> EditProject(Guid id, EditProjectRequestDto request, CancellationToken ct)
    {
        await _projectService.EditProject(id, request, ct);

        return Ok();
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> CreateProject(CreateProjectRequestDto request, CancellationToken ct)
    {
        await _projectService.CreateProject(request, ct);
        return Ok(new { message = "Project created successfully" });
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid id, CancellationToken ct)
    {
        await _projectService.DeleteProject(id, ct);

        return NoContent();
    }
}