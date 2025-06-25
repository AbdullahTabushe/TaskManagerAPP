using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class ProjectCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string Description { get; set; } = null!;
    }
}
