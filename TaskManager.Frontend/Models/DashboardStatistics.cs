namespace TaskManager.Frontend.Models
{
    public class DashboardStatistics
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int TasksAssignedToUser { get; set; }
        public int UpcomingDeadlines { get; set; }
        public int OverdueTasks { get; set; }
        public List<TaskReadDto> RecentTasks { get; set; } = new();
        public List<ProjectReadDto> RecentProjects { get; set; } = new();
    }

    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
    }
} 