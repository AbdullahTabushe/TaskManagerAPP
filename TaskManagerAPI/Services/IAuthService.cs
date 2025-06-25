using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserCreateDto userCreateDto);
    }
}
