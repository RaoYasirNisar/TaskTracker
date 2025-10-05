using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Web.Models;
using TaskTracker.Web.Services;

namespace TaskTracker.Web.Controllers;

[Authorize]
public class TasksBrowseController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<TasksBrowseController> _logger;

    public TasksBrowseController(IApiService apiService, ILogger<TasksBrowseController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(TaskFilterViewModel filter)
    {
        try
        {
            // Get all projects for the dropdown
            var projects = await _apiService.GetProjectsAsync();

            // Get filtered tasks
            var tasksResult = await _apiService.GetFilterTasksAsync(
                filter.Status,
                filter.DueDateBefore,
                filter.DueDateAfter,
                filter.ProjectId,
                filter.PageNumber,
                filter.PageSize);

            var viewModel = new TaskFilterViewModel
            {
                Status = filter.Status,
                DueDateBefore = filter.DueDateBefore,
                DueDateAfter = filter.DueDateAfter,
                ProjectId = filter.ProjectId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = tasksResult.TotalPages,
                TotalCount = tasksResult.TotalCount,
                Projects = projects,
                Tasks = tasksResult.Items
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tasks browse page");
            TempData["Error"] = "Error loading tasks";
            return BadRequest();
        }
    }
}