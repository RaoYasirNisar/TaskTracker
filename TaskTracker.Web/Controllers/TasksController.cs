using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Web.Services;

namespace TaskTracker.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(IApiService apiService, ILogger<TasksController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(string title, string? description, DateTime dueDate, string status, int projectId)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(status) || projectId == 0)
        {
            TempData["Error"] = "All fields are required";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var mappedStatus = MapStatus(status);

            await _apiService.CreateTaskAsync(new
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Status = mappedStatus,
                ProjectId = projectId
            });

            TempData["Success"] = "Task created successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create task";
        }

        return RedirectToAction("Index", "Home");
    }

    private TaskTracker.Core.Entities.TaskStatus MapStatus(string status)
    {
        return status switch
        {
            "Pending" => TaskTracker.Core.Entities.TaskStatus.Pending,
            "InProgress" => TaskTracker.Core.Entities.TaskStatus.InProgress,
            "Completed" => TaskTracker.Core.Entities.TaskStatus.Completed,
            _ => TaskTracker.Core.Entities.TaskStatus.Pending
        };
    }
    [HttpPost]
public async Task<IActionResult> Edit(int id, string title, string? description, DateTime dueDate, string status, int projectId)
{
    if (string.IsNullOrWhiteSpace(title))
    {
        TempData["Error"] = "Task title is required";
        return RedirectToAction("Index", "Home");
    }

    try
    {
        var mappedStatus = MapStatus(status);

            var result = await _apiService.UpdateTaskAsync(id, new
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            Status = mappedStatus,
            ProjectId = projectId
        });
        
        if (result.Success)
        {
            TempData["Success"] = "Task updated successfully";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Failed to update task";
        }

    }
        catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to update task");
        TempData["Error"] = "Failed to update task";
    }

    return RedirectToAction("Index", "Home");
}

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _apiService.DeleteTaskAsync(id);
            if (result.Success)
            {
                TempData["Success"] = "Task deleted successfully";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage ?? "Failed to delete task";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete task");
            TempData["Error"] = "Failed to delete task";
        }

        return RedirectToAction("Index", "Home");
    }

}