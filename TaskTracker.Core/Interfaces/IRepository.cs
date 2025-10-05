using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Interfaces;
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsWithTasksAsync();
}

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetTasksWithFilterAsync(
        Entities.TaskStatus? status,
        DateTime? dueDateBefore,
        DateTime? dueDateAfter,
        int? projectId,
        int pageNumber = 1,
        int pageSize = 10);

    Task<int> GetTotalCountAsync(Entities.TaskStatus? status, DateTime? dueDateBefore, DateTime? dueDateAfter, int? projectId);
}