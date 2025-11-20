using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ImageUploadExample.Models
{
    public class ImageService
    {
        private readonly string _imagesDirectory;

        public ImageService()
        {
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(_imagesDirectory);
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imagesDirectory, uniqueName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueName;
        }

        public void DeleteImage(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
            {
                return;
            }

            var path = Path.Combine(_imagesDirectory, imageFileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public async Task<string> ReplaceImageAsync(string oldFileName, IFormFile newFile)
        {
            DeleteImage(oldFileName);
            return await SaveImageAsync(newFile);
        }
    }
}
