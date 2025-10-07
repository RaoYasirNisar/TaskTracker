using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.API.DTOs;
using TaskTracker.Infrastructure.Data;
using Xunit;

namespace TaskTracker.API.Tests.Controllers;

public class ProjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _token;

    public ProjectsControllerTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                   // removing production database
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });

        _client = _factory.CreateClient();
        _token = GetJwtToken().Result;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    private async Task<string> GetJwtToken()
    {
        var loginRequest = new { Username = "admin", Password = "admin" };
        var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return authResponse?.Token ?? string.Empty;
        }
        return string.Empty;
    }

    [Fact]
    public async Task GetProjects_WithoutAuth_ShouldReturnUnauthorized()
    {
   
        var client = _factory.CreateClient(); // Client without token

       
        var response = await client.GetAsync("/api/projects");

      
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProjects_WithAuth_ShouldReturnOk()
    {
    
        var response = await _client.GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProject_WithValidData_ShouldReturnCreated()
    {
       
        var project = new CreateProjectDto { Name = "Integration Test Project", Description = "Test Description" };
        var content = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, "application/json");

       
        var response = await _client.PostAsync("/api/projects", content);

       
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdProject = JsonSerializer.Deserialize<ProjectDto>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(createdProject);
        Assert.Equal("Integration Test Project", createdProject.Name);
    }

    [Fact]
    public async Task CreateProject_WithInvalidData_ShouldReturnBadRequest()
    {
       
        var project = new CreateProjectDto { Name = "", Description = "Test Description" }; // Empty name
        var content = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, "application/json");

       
        var response = await _client.PostAsync("/api/projects", content);

       
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}