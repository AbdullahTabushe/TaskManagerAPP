using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Caching.Memory;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public TaskService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<TaskReadDto?> CreateTaskAsync(TaskCreateDto taskDto, int userId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == taskDto.ProjectId && p.UserId == userId);
            if (project == null)
            {
                return null;
            }

            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Status = "ToDo",
                ProjectId = taskDto.ProjectId,
                UserId = userId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id, userId, false);
        }

        public async Task<IEnumerable<TaskReadDto>> GetTasksByProjectAsync(int projectId, int userId, QueryParameters queryParameters)
        {
            var project = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
            if (project == null)
            {
                return Enumerable.Empty<TaskReadDto>();
            }

            var query = _context.Tasks.Where(t => t.ProjectId == projectId);

            if (!string.IsNullOrEmpty(queryParameters.FilterBy) && !string.IsNullOrEmpty(queryParameters.FilterQuery))
            {
                 query = query.Where($"{queryParameters.FilterBy}.Contains(@0)", queryParameters.FilterQuery);
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                var sortOrder = queryParameters.IsDescending ? "descending" : "ascending";
                query = query.OrderBy($"{queryParameters.SortBy} {sortOrder}");
            }
            
            query = query.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                         .Take(queryParameters.PageSize);

            return await query
                .Select(t => new TaskReadDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate,
                    AssignedTo = t.User != null ? t.User.Username : "Unassigned",
                    ProjectName = t.Project.Name
                }).ToListAsync();
        }

        public async Task<TaskReadDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            return await GetTaskByIdAsync(taskId, userId, true);
        }

        private async Task<TaskReadDto?> GetTaskByIdAsync(int taskId, int userId, bool useCache)
        {
            var cacheKey = $"task_{taskId}";
            if (useCache && _cache.TryGetValue(cacheKey, out TaskReadDto? taskDto))
            {
                return taskDto;
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
            {
                return null;
            }

            taskDto = new TaskReadDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                AssignedTo = task.User != null ? task.User.Username : "Unassigned",
                ProjectName = task.Project.Name
            };
            
            _cache.Set(cacheKey, taskDto, TimeSpan.FromMinutes(10));

            return taskDto;
        }

        public async Task<bool> UpdateTaskAsync(int taskId, TaskUpdateDto taskDto, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
            {
                return false;
            }

            if (taskDto.Title != null) task.Title = taskDto.Title;
            if (taskDto.Description != null) task.Description = taskDto.Description;
            if (taskDto.DueDate.HasValue) task.DueDate = taskDto.DueDate.Value;
            if (taskDto.Status != null) task.Status = taskDto.Status;
            if (taskDto.AssignedUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == taskDto.AssignedUserId.Value);
                if (!userExists) return false;
                task.UserId = taskDto.AssignedUserId.Value;
            }

            await _context.SaveChangesAsync();
            
            var cacheKey = $"task_{taskId}";
            _cache.Remove(cacheKey);

            return true;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            var cacheKey = $"task_{taskId}";
            _cache.Remove(cacheKey);

            return true;
        }
    }
}
