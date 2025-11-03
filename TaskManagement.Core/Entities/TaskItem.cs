namespace TaskManagement.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int ColumnId { get; set; }
        public bool IsFavorite { get; set; }
        public int Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public Column Column { get; set; } = null!;
        public ICollection<TaskImage> Images { get; set; } = new List<TaskImage>();
    }
}
