using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.Services
{
    public interface IProjectService
    {
        Task<ProjectReadDto?> CreateProjectAsync(ProjectCreateDto projectDto, int userId);
        Task<IEnumerable<ProjectReadDto>> GetProjectsAsync(int userId, QueryParameters queryParameters);
        Task<ProjectReadDto?> GetProjectByIdAsync(int projectId, int userId);
        Task<bool> UpdateProjectAsync(int projectId, ProjectCreateDto projectDto, int userId);
        Task<bool> DeleteProjectAsync(int projectId, int userId);
    }
}
