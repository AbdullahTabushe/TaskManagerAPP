using System.Net.Http.Json;
using TaskManager.Frontend.Models;

namespace TaskManager.Frontend.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Project operations
        public async Task<List<ProjectReadDto>> GetProjectsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProjectReadDto>>("api/v1/Project");
                return response ?? new List<ProjectReadDto>();
            }
            catch
            {
                return new List<ProjectReadDto>();
            }
        }

        public async Task<List<ProjectReadDto>> GetProjectsAsync(QueryParameters queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var response = await _httpClient.GetFromJsonAsync<List<ProjectReadDto>>($"api/v1/Project?{queryString}");
                return response ?? new List<ProjectReadDto>();
            }
            catch
            {
                return new List<ProjectReadDto>();
            }
        }

        public async Task<ProjectReadDto?> GetProjectByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProjectReadDto>($"api/v1/Project/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectReadDto?> CreateProjectAsync(ProjectCreateDto projectDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/Project", projectDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProjectReadDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateProjectAsync(int id, ProjectCreateDto projectDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/v1/Project/{id}", projectDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/v1/Project/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Task operations
        public async Task<List<TaskReadDto>> GetTasksByProjectAsync(int projectId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<TaskReadDto>>($"api/v1/Task/project/{projectId}");
                return response ?? new List<TaskReadDto>();
            }
            catch
            {
                return new List<TaskReadDto>();
            }
        }

        public async Task<List<TaskReadDto>> GetTasksByProjectAsync(int projectId, QueryParameters queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var response = await _httpClient.GetFromJsonAsync<List<TaskReadDto>>($"api/v1/Task/project/{projectId}?{queryString}");
                return response ?? new List<TaskReadDto>();
            }
            catch
            {
                return new List<TaskReadDto>();
            }
        }

        public async Task<TaskReadDto?> GetTaskByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TaskReadDto>($"api/v1/Task/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<TaskReadDto?> CreateTaskAsync(TaskCreateDto taskDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/v1/Task", taskDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TaskReadDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateTaskAsync(int id, TaskUpdateDto taskDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/v1/Task/{id}", taskDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/v1/Task/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // User management operations (Admin only)
        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<UserReadDto>>("api/v1/User");
                return response ?? new List<UserReadDto>();
            }
            catch
            {
                return new List<UserReadDto>();
            }
        }

        public async Task<UserReadDto?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserReadDto>($"api/v1/User/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserReadDto?> CreateUserAsync(UserCreateDto userDto)
        {
            try
            {
                Console.WriteLine($"ApiService: Sending user creation request - Username: {userDto.Username}, Role: {userDto.Role}");
                
                var response = await _httpClient.PostAsJsonAsync("api/v1/User", userDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserReadDto>();
                    Console.WriteLine($"ApiService: User created - ID: {result?.Id}, Username: {result?.Username}, Role: {result?.Role}");
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ApiService: User creation failed - {response.StatusCode}: {errorContent}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiService: Exception creating user - {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/v1/User/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Dashboard and Statistics
        public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
        {
            try
            {
                // Get projects owned by the user for the "Total Projects" stat
                var projects = await GetProjectsAsync();

                // Get tasks from projects the user owns
                var tasksFromMyProjects = new List<TaskReadDto>();
                foreach (var project in projects)
                {
                    var tasks = await GetTasksByProjectAsync(project.Id);
                    tasksFromMyProjects.AddRange(tasks);
                }

                // Get tasks assigned to the user
                var assignedTasks = await GetMyTasksAsync(new QueryParameters { PageSize = 500 });

                // Combine the lists and remove duplicates
                var allRelevantTasks = tasksFromMyProjects
                    .Concat(assignedTasks)
                    .GroupBy(t => t.Id)
                    .Select(g => g.First())
                    .ToList();
                
                return new DashboardStatistics
                {
                    TotalProjects = projects.Count,
                    TotalTasks = allRelevantTasks.Count,
                    PendingTasks = allRelevantTasks.Count(t => t.Status == "Pending" || t.Status == "ToDo"),
                    InProgressTasks = allRelevantTasks.Count(t => t.Status == "In Progress"),
                    CompletedTasks = allRelevantTasks.Count(t => t.Status == "Completed"),
                    UpcomingDeadlines = allRelevantTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value <= DateTime.Now.AddDays(3) && t.Status != "Completed"),
                    OverdueTasks = allRelevantTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != "Completed"),
                    RecentTasks = allRelevantTasks.OrderByDescending(t => t.Id).Take(5).ToList(),
                    RecentProjects = projects.OrderByDescending(p => p.Id).Take(5).ToList()
                };
            }
            catch
            {
                return new DashboardStatistics();
            }
        }

        // Activity Logs (placeholder for now)
        public async Task<List<ActivityLogDto>> GetActivityLogsAsync(string? entityType = null, int? entityId = null)
        {
            try
            {
                // TODO: Implement when backend endpoint is ready
                return new List<ActivityLogDto>();
            }
            catch
            {
                return new List<ActivityLogDto>();
            }
        }

        // Helper method to build query strings
        private string BuildQueryString(QueryParameters queryParams)
        {
            var parameters = new List<string>();

            if (queryParams.PageNumber > 1)
                parameters.Add($"pageNumber={queryParams.PageNumber}");
            
            if (queryParams.PageSize != 10)
                parameters.Add($"pageSize={queryParams.PageSize}");
            
            if (!string.IsNullOrEmpty(queryParams.SortBy))
                parameters.Add($"sortBy={Uri.EscapeDataString(queryParams.SortBy)}");
            
            if (queryParams.IsDescending)
                parameters.Add($"isDescending={queryParams.IsDescending}");
            
            if (!string.IsNullOrEmpty(queryParams.FilterQuery))
                parameters.Add($"filterQuery={Uri.EscapeDataString(queryParams.FilterQuery)}");
            
            if (!string.IsNullOrEmpty(queryParams.Status))
                parameters.Add($"status={Uri.EscapeDataString(queryParams.Status)}");

            return string.Join("&", parameters);
        }

        // My Tasks (assigned to current user)
        public async Task<List<TaskReadDto>> GetMyTasksAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<TaskReadDto>>("api/v1/Task/assigned");
                return response ?? new List<TaskReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiService: Error fetching my tasks - {ex.Message}");
                return new List<TaskReadDto>();
            }
        }

        public async Task<List<TaskReadDto>> GetMyTasksAsync(QueryParameters queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var response = await _httpClient.GetFromJsonAsync<List<TaskReadDto>>($"api/v1/Task/assigned?{queryString}");
                return response ?? new List<TaskReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiService: Error fetching my tasks with filters - {ex.Message}");
                return new List<TaskReadDto>();
            }
        }

        public async Task<List<TaskReadDto>> GetTasksAssignedToUserAsync(int userId, QueryParameters queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var response = await _httpClient.GetFromJsonAsync<List<TaskReadDto>>($"api/v1/Task/assigned/{userId}?{queryString}");
                return response ?? new List<TaskReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiService: Error fetching tasks assigned to user {userId} - {ex.Message}");
                return new List<TaskReadDto>();
            }
        }
    }
} 