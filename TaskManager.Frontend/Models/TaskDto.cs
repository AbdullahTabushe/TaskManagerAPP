using System.ComponentModel.DataAnnotations;

namespace TaskManager.Frontend.Models
{
    public class TaskCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }
    }

    public class TaskUpdateDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public int? AssignedUserId { get; set; }
    }

    public class TaskReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public int? AssignedUserId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
} 