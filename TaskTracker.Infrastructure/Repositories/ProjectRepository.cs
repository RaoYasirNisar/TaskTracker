using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;
public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context) => _context = context;

    public async Task<Project?> GetByIdAsync(int id) =>
        await _context.Projects.FindAsync(id);

    public async Task<IEnumerable<Project>> GetAllAsync() =>
        await _context.Projects.ToListAsync();

    public async Task<IEnumerable<Project>> GetProjectsWithTasksAsync() =>
        await _context.Projects.Include(p => p.Tasks).ToListAsync();

    public async Task<Project> AddAsync(Project entity)
    {
        _context.Projects.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task UpdateAsync(Project entity)
    {
        _context.Projects.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Project entity)
    {
        _context.Projects.Remove(entity);
        await _context.SaveChangesAsync();
    }
}