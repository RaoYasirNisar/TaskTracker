using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Entities;
using TaskTracker.Core.Interfaces;
using TaskStatus = TaskTracker.Core.Entities.TaskStatus;

namespace TaskTracker.Infrastructure.Data;
public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context, IAuthService authService)
    {
        if (await context.Users.AnyAsync())
            return;


        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@tasktracker.com",
            PasswordHash = authService.HashPassword("password"),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        // sample project
        var projects = new List<Project>
        {
            new() {
                Name = "Website Redesign",
                Description = "Complete overhaul of company website",
                UserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new() {
                Name = "Mobile App Development",
                Description = "Build cross-platform mobile application",
                UserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        context.Projects.AddRange(projects);
        await context.SaveChangesAsync();

        // sample task
        var tasks = new List<TaskItem>
        {
            new() {
                Title = "Design Homepage Mockup",
                Description = "Create initial design concepts for homepage",
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = TaskStatus.InProgress,
                ProjectId = projects[0].Id,
                UserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Title = "Setup Development Environment",
                Description = "Configure CI/CD pipeline and development tools",
                DueDate = DateTime.UtcNow.AddDays(3),
                Status = TaskStatus.Pending,
                ProjectId = projects[1].Id,
                UserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new() {
                Title = "User Authentication System",
                Description = "Implement JWT-based authentication",
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = TaskStatus.Completed,
                ProjectId = projects[1].Id,
                UserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        context.Tasks.AddRange(tasks);
        await context.SaveChangesAsync();
    }
}