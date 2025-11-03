namespace TaskManagement.Core.DTOs
{
    public class CreateTaskDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
    }
}
