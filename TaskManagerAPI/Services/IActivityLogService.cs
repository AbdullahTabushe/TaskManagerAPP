using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(string actionType, string description, int userId, string entityType, int entityId);
        Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(string? entityType = null, int? entityId = null);
        Task<IEnumerable<ActivityLog>> GetUserActivityLogsAsync(int userId);
    }
} 