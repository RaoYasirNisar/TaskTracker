using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Web.Services;

namespace TaskTracker.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<ProjectsController> _logger;
    public ProjectsController(IApiService apiService, ILogger<ProjectsController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Project name is required";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            await _apiService.CreateProjectAsync(new { Name = name, Description = description });
            TempData["Success"] = "Project created successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to create project";
        }

        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Project name is required";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            await _apiService.UpdateProjectAsync(id, new { Name = name, Description = description });
            TempData["Success"] = "Project updated successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update project");
            TempData["Error"] = "Failed to update project";
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _apiService.DeleteProjectAsync(id);
            TempData["Success"] = "Project deleted successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete project");
            TempData["Error"] = "Failed to delete project";
        }

        return RedirectToAction("Index", "Home");
    }
}