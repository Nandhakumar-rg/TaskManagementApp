namespace TaskManagement.Core.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
        public int Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<TaskImageDto> Images { get; set; } = new();
    }
}
