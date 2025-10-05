using TaskTracker.API.DTOs;
using TaskTracker.Web.Services;

namespace TaskTracker.Web.Models;

public class DashboardViewModel
{
    public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    public IEnumerable<API.DTOs.TaskDto> Tasks { get; set; } = new List<API.DTOs.TaskDto>();
    
    public int TotalProjects => Projects.Count();
    public int TotalTasks => Tasks.Count();
    public int CompletedTasks => Tasks.Count(t => t.Status == TaskTracker.Core.Entities.TaskStatus.Completed);
    public int InProgressTasks => Tasks.Count(t => t.Status == TaskTracker.Core.Entities.TaskStatus.InProgress);
    public int PendingTasks => Tasks.Count(t => t.Status == TaskTracker.Core.Entities.TaskStatus.Pending);
}

public class CreateProjectModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateTaskModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    public string Status { get; set; } = "Pending";
    public int ProjectId { get; set; }
}