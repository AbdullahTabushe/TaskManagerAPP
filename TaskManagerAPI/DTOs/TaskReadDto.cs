namespace TaskManagerAPI.DTOs
{
    public class TaskReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
    }
}
