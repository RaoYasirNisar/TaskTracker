using Microsoft.EntityFrameworkCore;
using TaskTracker.Core.Entities;
using TaskTracker.Infrastructure.Data;
using TaskTracker.Infrastructure.Repositories;
using Xunit;

namespace TaskTracker.Infrastructure.Tests.Repositories;

public class ProjectRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ProjectRepository _repository;

    public ProjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProject()
    {
  
        var project = new Project { Name = "Test Project", UserId = 1 };
     
        var result = await _repository.AddAsync(project);
        
        Assert.NotNull(result);
        Assert.Equal("Test Project", result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnProject()
    {   
        var project = new Project { Name = "Test Project", UserId = 1 };
        await _repository.AddAsync(project);
   
        var result = await _repository.GetByIdAsync(project.Id);

        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }
}