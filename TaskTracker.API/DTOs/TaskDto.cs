namespace TaskTracker.API.DTOs;
public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskTracker.Core.Entities.TaskStatus Status { get; set; } = TaskTracker.Core.Entities.TaskStatus.Pending;
    public int ProjectId { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskTracker.Core.Entities.TaskStatus Status { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TaskFilterDto
{
    public TaskTracker.Core.Entities.TaskStatus? Status { get; set; }
    public DateTime? DueDateBefore { get; set; }
    public DateTime? DueDateAfter { get; set; }
    public int? ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}