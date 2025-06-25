using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<TaskReadDto?> CreateTaskAsync(TaskCreateDto taskDto, int userId);
        Task<IEnumerable<TaskReadDto>> GetTasksByProjectAsync(int projectId, int userId, QueryParameters queryParameters);
        Task<TaskReadDto?> GetTaskByIdAsync(int taskId, int userId);
        Task<bool> UpdateTaskAsync(int taskId, TaskUpdateDto taskDto, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }
}
