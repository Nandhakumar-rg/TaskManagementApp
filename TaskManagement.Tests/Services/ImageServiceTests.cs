using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Tests.Services
{
    public class ImageServiceTests : IDisposable
    {
        private readonly TaskManagementDbContext _context;
        private readonly ImageService _service;

        public ImageServiceTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskManagementDbContext(options);
            _service = new ImageService(_context, null); // null for testing without blob storage

            SeedData();
        }

        private void SeedData()
        {
            var column = new Column { Id = 1, Name = "To Do", Order = 1 };
            var task = new TaskItem
            {
                Id = 1,
                Name = "Test Task",
                ColumnId = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Columns.Add(column);
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        [Fact]
        public async Task UploadImageAsync_ShouldCreateTaskImage()
        {
            // Arrange
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var fileName = "test.jpg";
            var contentType = "image/jpeg";

            // Act
            var result = await _service.UploadImageAsync(1, stream, fileName, contentType);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.TaskId.Should().Be(1);
            result.FileName.Should().Be(fileName);
            result.ContentType.Should().Be(contentType);
            result.ImageUrl.Should().Contain(fileName);
        }

        [Fact]
        public async Task GetTaskImagesAsync_ShouldReturnImagesForTask()
        {
            // Arrange
            var images = new[]
            {
                new TaskImage
                {
                    TaskId = 1,
                    ImageUrl = "url1",
                    BlobName = "blob1",
                    FileName = "file1.jpg",
                    ContentType = "image/jpeg",
                    UploadedDate = DateTime.UtcNow
                },
                new TaskImage
                {
                    TaskId = 1,
                    ImageUrl = "url2",
                    BlobName = "blob2",
                    FileName = "file2.jpg",
                    ContentType = "image/jpeg",
                    UploadedDate = DateTime.UtcNow.AddMinutes(1)
                }
            };
            _context.TaskImages.AddRange(images);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _service.GetTaskImagesAsync(1)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].FileName.Should().Be("file1.jpg");
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldDeleteImage_WhenExists()
        {
            // Arrange
            var image = new TaskImage
            {
                TaskId = 1,
                ImageUrl = "url",
                BlobName = "blob",
                FileName = "file.jpg",
                ContentType = "image/jpeg",
                UploadedDate = DateTime.UtcNow
            };
            _context.TaskImages.Add(image);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteImageAsync(image.Id);

            // Assert
            result.Should().BeTrue();
            var deletedImage = await _context.TaskImages.FindAsync(image.Id);
            deletedImage.Should().BeNull();
        }

        [Fact]
        public async Task DeleteImageAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Act
            var result = await _service.DeleteImageAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}