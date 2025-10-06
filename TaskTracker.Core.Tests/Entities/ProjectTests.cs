using TaskTracker.Core.Entities;
using Xunit;

namespace TaskTracker.Core.Tests.Entities;

public class ProjectTests
{
    [Fact]
    public void Project_Create_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "Test Description",
            UserId = 1
        };

        // Assert
        Assert.Equal("Test Project", project.Name);
        Assert.Equal("Test Description", project.Description);
        Assert.Equal(1, project.UserId);
        Assert.NotNull(project.Tasks);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Project_WithInvalidName_ShouldThrowException(string invalidName)
    {
        Assert.ThrowsAny<Exception>(() => new Project { Name = invalidName });
    }
}