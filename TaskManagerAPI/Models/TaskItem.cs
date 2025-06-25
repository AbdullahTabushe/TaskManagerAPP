namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "ToDo";

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
