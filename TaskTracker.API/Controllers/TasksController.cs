using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.API.DTOs;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository, ILogger<TasksController> logger)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks([FromQuery] TaskFilterDto filter)
    {
        var tasks = await _taskRepository.GetTasksWithFilterAsync(
            filter.Status, filter.DueDateBefore, filter.DueDateAfter,
            filter.ProjectId, filter.PageNumber, filter.PageSize);

        var totalCount = await _taskRepository.GetTotalCountAsync(
            filter.Status, filter.DueDateBefore, filter.DueDateAfter, filter.ProjectId);

        var result = new PagedResult<TaskDto>
        {
            Items = tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Status = t.Status,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                CreatedAt = t.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Ok(result);
    }
    
    [HttpGet("filtered")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
    [FromQuery] TaskTracker.Core.Entities.TaskStatus? status,
    [FromQuery] DateTime? dueDateBefore,
    [FromQuery] DateTime? dueDateAfter,
    [FromQuery] int? projectId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var tasks = await _taskRepository.GetTasksWithFilterAsync(
                status, dueDateBefore, dueDateAfter, projectId, pageNumber, pageSize);

            var totalCount = await _taskRepository.GetTotalCountAsync(
                status, dueDateBefore, dueDateAfter, projectId);

            var result = new PagedResult<TaskDto>
            {
                Items = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Status = t.Status,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    CreatedAt = t.CreatedAt
                }),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return Problem("An error occurred while retrieving tasks");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var project = await _projectRepository.GetByIdAsync(createDto.ProjectId);
            if (project == null)
                return BadRequest("Invalid project ID");

            var task = new TaskItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                Status = createDto.Status,
                ProjectId = createDto.ProjectId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _taskRepository.AddAsync(task);

            var result = new TaskDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                DueDate = created.DueDate,
                Status = created.Status,
                ProjectId = created.ProjectId,
                ProjectName = project.Name,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetTasks), result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return Problem("An error occurred while creating the task");
        }
    }
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskDto updateDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
                return NotFound();

            if (task.UserId != userId)
                if (task.UserId != userId)
                    return Problem("You can only update your own tasks", statusCode: 403);

            var project = await _projectRepository.GetByIdAsync(updateDto.ProjectId);
            if (project == null)
                return BadRequest("Invalid project ID");

            task.Title = updateDto.Title;
            task.Description = updateDto.Description;
            task.DueDate = updateDto.DueDate;
            task.Status = updateDto.Status;
            task.ProjectId = updateDto.ProjectId;

            await _taskRepository.UpdateAsync(task);

            var result = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                ProjectId = task.ProjectId,
                ProjectName = project.Name,
                CreatedAt = task.CreatedAt
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task");
            return Problem("An error occurred while updating the task");
        }
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
                return NotFound();
       
            if (task.UserId != userId)
                return Problem("You can only delete your own tasks", statusCode: 403);

            await _taskRepository.DeleteAsync(task);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task");
            return Problem("An error occurred while deleting the task");
        }
    }
}