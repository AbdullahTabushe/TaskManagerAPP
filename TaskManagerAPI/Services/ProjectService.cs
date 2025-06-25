using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Caching.Memory;

namespace TaskManagerAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ProjectService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProjectReadDto?> CreateProjectAsync(ProjectCreateDto projectDto, int userId)
        {
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                UserId = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            
            var projectDtoResult = new ProjectReadDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            var cacheKey = $"project_{project.Id}";
            _cache.Set(cacheKey, projectDtoResult, TimeSpan.FromMinutes(10));

            return projectDtoResult;
        }

        public async Task<IEnumerable<ProjectReadDto>> GetProjectsAsync(int userId, QueryParameters queryParameters)
        {
            // Caching for paginated lists is complex, skipping for now.
            var query = _context.Projects
                .Where(p => p.UserId == userId);

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
                .Select(p => new ProjectReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToListAsync();
        }

        public async Task<ProjectReadDto?> GetProjectByIdAsync(int projectId, int userId)
        {
            var cacheKey = $"project_{projectId}";
            if (_cache.TryGetValue(cacheKey, out ProjectReadDto? projectDto))
            {
                return projectDto;
            }

            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                return null;
            }
            
            projectDto = new ProjectReadDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };
            
            _cache.Set(cacheKey, projectDto, TimeSpan.FromMinutes(10));
            return projectDto;
        }

        public async Task<bool> UpdateProjectAsync(int projectId, ProjectCreateDto projectDto, int userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                return false;
            }

            project.Name = projectDto.Name;
            project.Description = projectDto.Description;

            await _context.SaveChangesAsync();
            
            var cacheKey = $"project_{projectId}";
            _cache.Remove(cacheKey);

            return true;
        }

        public async Task<bool> DeleteProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            var cacheKey = $"project_{projectId}";
            _cache.Remove(cacheKey);

            return true;
        }
    }
}

