using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskTracker.API.DTOs;
using static TaskTracker.Web.Services.ApiService;

namespace TaskTracker.Web.Services;
public interface IApiService
{
    Task<IEnumerable<ProjectDto>> GetProjectsAsync();
    Task<PagedResult<TaskDto>> GetTasksAsync();
    Task<PagedResult<TaskDto>> GetFilterTasksAsync(TaskTracker.Core.Entities.TaskStatus? status = null, DateTime? dueDateBefore = null, DateTime? dueDateAfter = null, int? projectId = null, int pageNumber = 1, int pageSize = 10);
    Task CreateProjectAsync(object project);
    Task CreateTaskAsync(object task);
    Task<string> LoginAsync(string username, string password);
    Task<RegisterResponse> RegisterAsync(string username, string email, string password);
    Task UpdateProjectAsync(int id, object project);
    Task DeleteProjectAsync(int id);
    Task UpdateTaskAsync(int id, object task);
    Task DeleteTaskAsync(int id);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]!);
    }

    private async Task AddAuthHeaderAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.GetAsync("/api/projects");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProjectDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProjectDto>();
        }

        return new List<ProjectDto>();
    }

    public async Task<PagedResult<TaskDto>> GetTasksAsync()
    {
        var response = await _httpClient.GetAsync("/api/tasks");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedResult<TaskDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PagedResult<TaskDto>();
        }

        return new PagedResult<TaskDto>();
    }
    public async Task<PagedResult<TaskDto>> GetFilterTasksAsync(TaskTracker.Core.Entities.TaskStatus? status = null, DateTime? dueDateBefore = null, DateTime? dueDateAfter = null, int? projectId = null, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            // Build query string
            var queryParams = new List<string>();

            if (status.HasValue)
                queryParams.Add($"status={status.Value}");

            if (dueDateBefore.HasValue)
                queryParams.Add($"dueDateBefore={dueDateBefore.Value:yyyy-MM-dd}");

            if (dueDateAfter.HasValue)
                queryParams.Add($"dueDateAfter={dueDateAfter.Value:yyyy-MM-dd}");

            if (projectId.HasValue)
                queryParams.Add($"projectId={projectId.Value}");

            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync($"/api/tasks/filtered/{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PagedResult<TaskDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PagedResult<TaskDto>();                
                return new PagedResult<TaskDto>
                {
                    Items = result.Items,
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize                    
                };
            }

            return new PagedResult<TaskDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in GetTasksAsync: {ex.Message}");
            return new PagedResult<TaskDto>();
        }
    }
    public async Task CreateProjectAsync(object project)
    {
        await AddAuthHeaderAsync();
        var content = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/projects", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateTaskAsync(object task)
    {
        await AddAuthHeaderAsync();
        var content = new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/tasks", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var loginData = new { Username = username, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<LoginResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return tokenResponse?.Token ?? string.Empty;
        }

        return string.Empty;
    }
    public async Task<RegisterResponse> RegisterAsync(string username, string email, string password)
    {
        var registerData = new { Username = username, Email = email, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RegisterResponse>(result,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new RegisterResponse();
        }

        throw new HttpRequestException($"Registration failed: {response.StatusCode}");
    }
    public async Task UpdateProjectAsync(int id, object project)
    {
        await AddAuthHeaderAsync();
        var content = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/api/projects/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProjectAsync(int id)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"/api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateTaskAsync(int id, object task)
    {
        await AddAuthHeaderAsync();
        var content = new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/api/tasks/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTaskAsync(int id)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"/api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
    public class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}

// DTO classes for Web project
//public class ProjectDto
//{
//    public int Id { get; set; }
//    public string Name { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public int TaskCount { get; set; }
//}

//public class TaskDto
//{
//    public int Id { get; set; }
//    public string Title { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public DateTime DueDate { get; set; }
//    public TaskTracker.Core.Entities.TaskStatus Status { get; set; }
//    public int ProjectId { get; set; }
//    public string ProjectName { get; set; } = string.Empty;
//    public DateTime CreatedAt { get; set; }
//}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}