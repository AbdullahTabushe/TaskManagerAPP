using TaskManager.Frontend.Models;

namespace TaskManager.Frontend.Services
{
    public interface IApiService
    {
        // Project operations
        Task<List<ProjectReadDto>> GetProjectsAsync();
        Task<List<ProjectReadDto>> GetProjectsAsync(QueryParameters queryParams);
        Task<ProjectReadDto?> GetProjectByIdAsync(int id);
        Task<ProjectReadDto?> CreateProjectAsync(ProjectCreateDto projectDto);
        Task<bool> UpdateProjectAsync(int id, ProjectCreateDto projectDto);
        Task<bool> DeleteProjectAsync(int id);

        // Task operations
        Task<List<TaskReadDto>> GetTasksByProjectAsync(int projectId);
        Task<List<TaskReadDto>> GetTasksByProjectAsync(int projectId, QueryParameters queryParams);
        Task<TaskReadDto?> GetTaskByIdAsync(int id);
        Task<TaskReadDto?> CreateTaskAsync(TaskCreateDto taskDto);
        Task<bool> UpdateTaskAsync(int id, TaskUpdateDto taskDto);
        Task<bool> DeleteTaskAsync(int id);

        // User management operations (Admin only)
        Task<List<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(int id);
        Task<UserReadDto?> CreateUserAsync(UserCreateDto userDto);
        Task<bool> DeleteUserAsync(int id);

        // Dashboard and Statistics
        Task<DashboardStatistics> GetDashboardStatisticsAsync();
        
        // Activity Logs
        Task<List<ActivityLogDto>> GetActivityLogsAsync(string? entityType = null, int? entityId = null);

        // My Tasks (assigned to current user)
        Task<List<TaskReadDto>> GetMyTasksAsync();
        Task<List<TaskReadDto>> GetMyTasksAsync(QueryParameters queryParams);
        Task<List<TaskReadDto>> GetTasksAssignedToUserAsync(int userId, QueryParameters queryParams);
    }
} 