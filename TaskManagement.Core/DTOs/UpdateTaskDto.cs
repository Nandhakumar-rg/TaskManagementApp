namespace TaskManagement.Core.DTOs
{
    public class UpdateTaskDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int? ColumnId { get; set; }
        public bool? IsFavorite { get; set; }
    }
}
