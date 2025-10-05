using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Web.Models;
using TaskTracker.Web.Services;

namespace TaskTracker.Web.Controllers;

public class HomeController : Controller
{
    private readonly IApiService _apiService;

    public HomeController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var projects = await _apiService.GetProjectsAsync();
            var tasks = await _apiService.GetTasksAsync();

            var viewModel = new DashboardViewModel
            {
                Projects = projects,
                Tasks = tasks.Items
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // If API is not available, return empty view model
            var viewModel = new DashboardViewModel();
            return View(viewModel);
        }
    }
}