using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Security.Claims;
using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDto projectDto)
        {
            var createdProject = await _projectService.CreateProjectAsync(projectDto, GetUserId());
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProject!.Id, version = "1.0" }, createdProject);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] QueryParameters queryParameters)
        {
            var projects = await _projectService.GetProjectsAsync(GetUserId(), queryParameters);
            return Ok(projects);
        }

        /// <summary>
        /// Gets a specific project by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The requested project.</returns>
        /// <response code="200">Returns the requested project.</response>
        /// <response code="404">If the project is not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id, GetUserId());
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectCreateDto projectDto)
        {
            var result = await _projectService.UpdateProjectAsync(id, projectDto, GetUserId());
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProjectAsync(id, GetUserId());
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
