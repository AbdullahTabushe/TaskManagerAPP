using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using BCrypt.Net;

namespace TaskManagerAPI.Services
{
    public enum DeleteResult { Success, NotFound, HasProjects }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserCreateDto userCreateDto)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Username == userCreateDto.Username);
            if (existingUser)
            {
                return null; 
            }

            var user = new User
            {
                Username = userCreateDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password),
                Role = userCreateDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponseDto { Token = token, Role = user.Role };
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role
            };
        }

        public async Task<DeleteResult> DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return DeleteResult.NotFound;
            }

            var hasProjects = await _context.Projects.AnyAsync(p => p.UserId == user.Id);
            if (hasProjects)
            {
                return DeleteResult.HasProjects;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return DeleteResult.Success;
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
