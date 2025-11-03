using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly TaskManagementDbContext _context;
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly string _containerName = "task-images";

        public ImageService(TaskManagementDbContext context, string? blobConnectionString = null)
        {
            _context = context;
            
            if (!string.IsNullOrEmpty(blobConnectionString))
            {
                _blobServiceClient = new BlobServiceClient(blobConnectionString);
            }
        }

        public async Task<TaskImage> UploadImageAsync(int taskId, Stream imageStream, string fileName, string contentType)
        {
            string blobName = $"{taskId}/{Guid.NewGuid()}-{fileName}";
            string imageUrl;

            if (_blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = contentType });

                imageUrl = blobClient.Uri.ToString();
            }
            else
            {
                // For testing without Azure Blob Storage
                imageUrl = $"https://localhost/images/{blobName}";
            }

            var taskImage = new TaskImage
            {
                TaskId = taskId,
                ImageUrl = imageUrl,
                BlobName = blobName,
                FileName = fileName,
                ContentType = contentType,
                UploadedDate = DateTime.UtcNow
            };

            _context.TaskImages.Add(taskImage);
            await _context.SaveChangesAsync();

            return taskImage;
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _context.TaskImages.FindAsync(imageId);
            if (image == null)
                return false;

            if (_blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(image.BlobName);
                await blobClient.DeleteIfExistsAsync();
            }

            _context.TaskImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskImage>> GetTaskImagesAsync(int taskId)
        {
            return await _context.TaskImages
                .Where(i => i.TaskId == taskId)
                .OrderBy(i => i.UploadedDate)
                .ToListAsync();
        }
    }
}