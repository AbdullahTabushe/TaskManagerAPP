using TaskManager.Frontend.Models;

namespace TaskManager.Frontend.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserCreateDto createDto);
        Task LogoutAsync();
        Task<bool> IsLoggedInAsync();
        Task<string?> GetTokenAsync();
        Task<string?> GetUserRoleAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetUsernameAsync();
    }
} 