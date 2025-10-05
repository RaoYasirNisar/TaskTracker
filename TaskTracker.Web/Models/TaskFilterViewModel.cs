using TaskTracker.API.DTOs;

namespace TaskTracker.Web.Models;

public class TaskFilterViewModel
{
    public TaskTracker.Core.Entities.TaskStatus? Status { get; set; }
    public DateTime? DueDateBefore { get; set; }
    public DateTime? DueDateAfter { get; set; }
    public int? ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    // For dropdowns
    public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    public IEnumerable<TaskDto> Tasks { get; set; } = new List<TaskDto>();

    // Status options for dropdown
    public List<SelectListItem> StatusOptions => new()
    {
        new SelectListItem { Value = "", Text = "All Statuses" },
        new SelectListItem { Value = "0", Text = "Pending" },
        new SelectListItem { Value = "1", Text = "In Progress" },
        new SelectListItem { Value = "2", Text = "Completed" }
    };
}

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
}