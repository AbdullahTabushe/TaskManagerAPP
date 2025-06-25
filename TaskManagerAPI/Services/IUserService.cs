using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface IUserService
    {
        Task<UserReadDto> CreateUserAsync(UserCreateDto userDto);
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}
