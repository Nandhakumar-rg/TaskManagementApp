using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ITaskRepository _taskRepository;

        public ImagesController(IImageService imageService, ITaskRepository taskRepository)
        {
            _imageService = imageService;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskImageDto>>> GetTaskImages(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return NotFound("Task not found");

            var images = await _imageService.GetTaskImagesAsync(taskId);
            var imageDtos = images.Select(i => new TaskImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                FileName = i.FileName,
                UploadedDate = i.UploadedDate
            }).ToList();

            return Ok(imageDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TaskImageDto>> UploadImage(int taskId, IFormFile file)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return NotFound("Task not found");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Invalid file type. Only JPEG, PNG, and GIF are allowed");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                return BadRequest("File size exceeds 5MB limit");

            using var stream = file.OpenReadStream();
            var image = await _imageService.UploadImageAsync(taskId, stream, file.FileName, file.ContentType);

            var imageDto = new TaskImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                FileName = image.FileName,
                UploadedDate = image.UploadedDate
            };

            return CreatedAtAction(nameof(GetTaskImages), new { taskId }, imageDto);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int taskId, int imageId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return NotFound("Task not found");

            var result = await _imageService.DeleteImageAsync(imageId);
            if (!result)
                return NotFound("Image not found");

            return NoContent();
        }
    }
}