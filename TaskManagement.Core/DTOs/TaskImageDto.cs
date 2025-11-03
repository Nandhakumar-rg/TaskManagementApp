namespace TaskManagement.Core.DTOs
{
    public class TaskImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedDate { get; set; }
    }
}
