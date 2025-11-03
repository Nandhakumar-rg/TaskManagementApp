using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Tests.Controllers
{
    public class ImagesControllerTests
    {
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<ITaskRepository> _mockTaskRepo;
        private readonly ImagesController _controller;

        public ImagesControllerTests()
        {
            _mockImageService = new Mock<IImageService>();
            _mockTaskRepo = new Mock<ITaskRepository>();
            _controller = new ImagesController(_mockImageService.Object, _mockTaskRepo.Object);
        }

        [Fact]
        public async Task GetTaskImages_ShouldReturnOk_WhenTaskExists()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Name = "Task" };
            var images = new List<TaskImage>
            {
                new TaskImage
                {
                    Id = 1,
                    TaskId = 1,
                    ImageUrl = "url",
                    FileName = "image.jpg",
                    UploadedDate = DateTime.UtcNow
                }
            };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _mockImageService.Setup(s => s.GetTaskImagesAsync(1)).ReturnsAsync(images);

            // Act
            var result = await _controller.GetTaskImages(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedImages = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskImageDto>>().Subject;
            returnedImages.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetTaskImages_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _controller.GetTaskImages(999);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Task not found");
        }

        [Fact]
        public async Task UploadImage_ShouldReturnBadRequest_WhenNoFileProvided()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Name = "Task" };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

            // Act
            var result = await _controller.UploadImage(1, null!);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("No file uploaded");
        }

        [Fact]
        public async Task UploadImage_ShouldReturnBadRequest_WhenInvalidFileType()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Name = "Task" };
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.Length).Returns(100);

            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

            // Act
            var result = await _controller.UploadImage(1, fileMock.Object);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Invalid file type. Only JPEG, PNG, and GIF are allowed");
        }

        [Fact]
        public async Task DeleteImage_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Name = "Task" };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _mockImageService.Setup(s => s.DeleteImageAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteImage(1, 1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteImage_ShouldReturnNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Name = "Task" };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _mockImageService.Setup(s => s.DeleteImageAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteImage(1, 999);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Image not found");
        }
    }
}