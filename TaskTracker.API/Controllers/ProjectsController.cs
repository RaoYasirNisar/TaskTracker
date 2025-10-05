using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.API.DTOs;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectRepository projectRepository, IUserRepository userRepository, ILogger<ProjectsController> logger)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var project = new Project
            {
                Name = createDto.Name,
                Description = createDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _projectRepository.AddAsync(project);

            var result = new ProjectDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreatedAt = created.CreatedAt,
                TaskCount = 0
            };

            return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return Problem("An error occurred while creating the project");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await _projectRepository.GetProjectsWithTasksAsync();
        var result = projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            TaskCount = p.Tasks.Count
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectDto>> GetProject(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return NotFound();

        var result = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };

        return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(int id, UpdateProjectDto updateDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                return NotFound();

            if (project.UserId != userId)
                return Forbid("You can only update your own projects");

            project.Name = updateDto.Name;
            project.Description = updateDto.Description;

            await _projectRepository.UpdateAsync(project);

            var result = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                TaskCount = project.Tasks.Count
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project");
            return Problem("An error occurred while updating the project");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                return NotFound();

            if (project.UserId != userId)
                return Forbid("You can only delete your own projects");

            await _projectRepository.DeleteAsync(project);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project");
            return Problem("An error occurred while deleting the project");
        }
    }
}