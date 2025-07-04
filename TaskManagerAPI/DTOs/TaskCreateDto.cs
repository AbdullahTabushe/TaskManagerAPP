using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [FutureDate(ErrorMessage = "Due date must be in the future.")]
        public DateTime DueDate { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }
    }
}
