namespace TaskManagerAPI.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string ActionType { get; set; } = null!; // Create, Update, Delete
        public string Description { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string EntityType { get; set; } = null!; // Project, Task, User
        public int EntityId { get; set; }
    }
} 