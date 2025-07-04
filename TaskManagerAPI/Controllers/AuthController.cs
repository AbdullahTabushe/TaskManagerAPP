using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserCreateDto userCreateDto)
        {
            var authResponse = await _authService.RegisterAsync(userCreateDto);

            if (authResponse == null)
            {
                return BadRequest("Username already exists.");
            }

            return Ok(authResponse);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto);
            if (authResponse == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(authResponse);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _authService.DeleteUserAsync(userId);

            return result switch
            {
                DeleteResult.Success => NoContent(),
                DeleteResult.NotFound => NotFound("User not found."),
                DeleteResult.HasProjects => BadRequest("User cannot be deleted because they have associated projects."),
                _ => StatusCode(500, "An unexpected error occurred.")
            };
        }
    }
}
