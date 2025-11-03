using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IImageService
    {
        Task<TaskImage> UploadImageAsync(int taskId, Stream imageStream, string fileName, string contentType);
        Task<bool> DeleteImageAsync(int imageId);
        Task<IEnumerable<TaskImage>> GetTaskImagesAsync(int taskId);
    }
}