using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context) => _context = context;

    public async Task<TaskItem?> GetByIdAsync(int id) =>
        await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<TaskItem>> GetAllAsync() =>
        await _context.Tasks.Include(t => t.Project).ToListAsync();

    public async Task<TaskItem> AddAsync(TaskItem entity)
    {
        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TaskItem entity)
    {
        _context.Tasks.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem entity)
    {
        _context.Tasks.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksWithFilterAsync(
        Core.Entities.TaskStatus? status,
        DateTime? dueDateBefore,
        DateTime? dueDateAfter,
        int? projectId,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _context.Tasks.Include(t => t.Project).AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (dueDateBefore.HasValue)
            query = query.Where(t => t.DueDate <= dueDateBefore.Value);

        if (dueDateAfter.HasValue)
            query = query.Where(t => t.DueDate >= dueDateAfter.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query.OrderBy(t => t.DueDate)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(Core.Entities.TaskStatus? status, DateTime? dueDateBefore, DateTime? dueDateAfter, int? projectId)
    {
        var query = _context.Tasks.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (dueDateBefore.HasValue)
            query = query.Where(t => t.DueDate <= dueDateBefore.Value);

        if (dueDateAfter.HasValue)
            query = query.Where(t => t.DueDate >= dueDateAfter.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query.CountAsync();
    }
}