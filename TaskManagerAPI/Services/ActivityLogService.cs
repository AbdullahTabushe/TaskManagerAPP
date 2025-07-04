using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _context;

        public ActivityLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string actionType, string description, int userId, string entityType, int entityId)
        {
            var activityLog = new ActivityLog
            {
                ActionType = actionType,
                Description = description,
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(string? entityType = null, int? entityId = null)
        {
            var query = _context.ActivityLogs
                .Include(al => al.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(entityType))
            {
                query = query.Where(al => al.EntityType == entityType);
            }

            if (entityId.HasValue)
            {
                query = query.Where(al => al.EntityId == entityId.Value);
            }

            return await query
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetUserActivityLogsAsync(int userId)
        {
            return await _context.ActivityLogs
                .Include(al => al.User)
                .Where(al => al.UserId == userId)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }
    }
} 