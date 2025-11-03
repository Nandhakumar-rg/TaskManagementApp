namespace TaskManagement.Core.Entities
{
    public class TaskImage
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
        
        public TaskItem Task { get; set; } = null!;
    }
}
