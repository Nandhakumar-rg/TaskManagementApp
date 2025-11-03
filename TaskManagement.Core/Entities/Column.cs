
namespace TaskManagement.Core.Entities
{
    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}