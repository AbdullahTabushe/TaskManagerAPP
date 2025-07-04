namespace TaskManagerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;

        // Navigation
        public List<TaskItem> Tasks { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
    }
}
