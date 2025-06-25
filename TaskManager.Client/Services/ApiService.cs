using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskManager.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private string? _jwt;
        private ClaimsPrincipal? _user;
        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;
        public bool IsAdmin => _user?.IsInRole("Admin") ?? false;
        public string? Username => _user?.Identity?.Name;
        public string? Role => _user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        private void DecodeJwt(string? jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                _user = null;
                return;
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var identity = new ClaimsIdentity(token.Claims, "jwt");
            _user = new ClaimsPrincipal(identity);
        }

        public void Logout()
        {
            _jwt = null;
            _user = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }

        // --- Auth ---
        public async Task<bool> LoginAsync(string username, string password)
        {
            var res = await _http.PostAsJsonAsync("api/v1/auth/login", new { username, password });
            if (!res.IsSuccessStatusCode) return false;
            var json = await res.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            _jwt = doc.RootElement.GetProperty("token").GetString();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
            DecodeJwt(_jwt);
            return true;
        }
        public async Task<bool> RegisterAsync(string username, string password)
        {
            var res = await _http.PostAsJsonAsync("api/v1/auth/register", new { username, password });
            return res.IsSuccessStatusCode;
        }

        // --- Projects ---
        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            return await _http.GetFromJsonAsync<List<ProjectDto>>("api/v1/project") ?? new();
        }
        public async Task<ProjectDto?> CreateProjectAsync(string name, string desc)
        {
            var res = await _http.PostAsJsonAsync("api/v1/project", new { name, description = desc });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ProjectDto>();
        }
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/v1/project/{id}");
            return res.IsSuccessStatusCode;
        }

        // --- Tasks ---
        public async Task<List<TaskDto>> GetTasksAsync(int projectId)
        {
            return await _http.GetFromJsonAsync<List<TaskDto>>($"api/v1/task/project/{projectId}") ?? new();
        }
        public async Task<TaskDto?> CreateTaskAsync(int projectId, string title, string desc, DateTime due, string status)
        {
            var res = await _http.PostAsJsonAsync("api/v1/task", new { title, description = desc, dueDate = due, projectId, status });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<TaskDto>();
        }
        public async Task<bool> UpdateTaskStatusAsync(int taskId, string status)
        {
            var res = await _http.PutAsJsonAsync($"api/v1/task/{taskId}", new { status });
            return res.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var res = await _http.DeleteAsync($"api/v1/task/{taskId}");
            return res.IsSuccessStatusCode;
        }

        // --- Admin User Management ---
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<UserDto>>("api/v1/user") ?? new();
        }
        public async Task<UserDto?> CreateUserAsync(string username, string password, string role)
        {
            var res = await _http.PostAsJsonAsync("api/v1/user", new { username, password, role });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<UserDto>();
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/v1/user/{id}");
            return res.IsSuccessStatusCode;
        }

        // --- DTOs ---
        public class ProjectDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
        public class TaskDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime DueDate { get; set; }
        }
        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
} 