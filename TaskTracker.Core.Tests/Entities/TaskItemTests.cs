using TaskTracker.Core.Entities;
using Xunit;
using TaskStatus = TaskTracker.Core.Entities.TaskStatus;

namespace TaskTracker.Core.Tests.Entities;

public class TaskItemTests
{
    [Fact]
    public void TaskItem_Create_ShouldInitializeWithDefaultValues()
    {
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            ProjectId = 1,
            UserId = 1
        };
 
        Assert.Equal("Test Task", task.Title);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.True(task.DueDate > DateTime.MinValue);
    }

    [Fact]
    public void TaskItem_WithPastDueDate_ShouldBeOverdue()
    {  
        var task = new TaskItem
        {
            Title = "Overdue Task",
            DueDate = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Pending
        };

        Assert.True(task.DueDate < DateTime.Now);
    }
}