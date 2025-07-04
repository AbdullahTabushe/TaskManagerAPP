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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto taskDto)
        {
            var createdTask = await _taskService.CreateTaskAsync(taskDto, GetUserId());
            if (createdTask == null)
            {
                return Forbid();
            }
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id, version = "1.0" }, createdTask);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId, [FromQuery] QueryParameters queryParameters)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId, GetUserId(), queryParameters);
            return Ok(tasks);
        }

        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedTasks([FromQuery] QueryParameters queryParameters)
        {
            var userId = GetUserId();
            Console.WriteLine($"DEBUG: GetAssignedTasks called for userId: {userId}");
            
            var tasks = await _taskService.GetTasksAssignedToUserAsync(userId, queryParameters);
            Console.WriteLine($"DEBUG: Found {tasks.Count()} tasks for user {userId}");
            
            return Ok(tasks);
        }

        [HttpGet("assigned/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTasksAssignedToUser(int userId, [FromQuery] QueryParameters queryParameters)
        {
            var tasks = await _taskService.GetTasksAssignedToUserAsync(userId, queryParameters);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, GetUserId());
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto taskDto)
        {
            var result = await _taskService.UpdateTaskAsync(id, taskDto, GetUserId());
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id, GetUserId());
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("debug/all-tasks")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasksDebug()
        {
            var tasks = await _taskService.GetAllTasksForDebugAsync();
            return Ok(tasks);
        }

        [HttpGet("debug/user-info")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userId = GetUserId();
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            return Ok(new { 
                UserId = userId, 
                Username = username, 
                Role = role 
            });
        }
    }
}
