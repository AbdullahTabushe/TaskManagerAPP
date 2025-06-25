using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = null!;
    }
}
