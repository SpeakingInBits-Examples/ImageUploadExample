using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

        public async Task<string> SaveImageAsync(IFormFile file, int? maxWidth = null, int? maxHeight = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imagesDirectory, uniqueName);

            // If no resizing requested, save directly
            if (!maxWidth.HasValue && !maxHeight.HasValue)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                return uniqueName;
            }

            // Resize while maintaining aspect ratio using ImageSharp
            using (var inStream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(inStream))
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                double widthRatio = maxWidth.HasValue ? (double)maxWidth.Value / originalWidth : double.PositiveInfinity;
                double heightRatio = maxHeight.HasValue ? (double)maxHeight.Value / originalHeight : double.PositiveInfinity;

                // choose the smaller ratio to fit within both constraints
                double ratio = Math.Min(widthRatio, heightRatio);

                // Prevent upscaling: only resize if ratio < 1
                int targetWidth = originalWidth;
                int targetHeight = originalHeight;
                if (ratio < 1 && !double.IsInfinity(ratio))
                {
                    targetWidth = Math.Max(1, (int)Math.Round(originalWidth * ratio));
                    targetHeight = Math.Max(1, (int)Math.Round(originalHeight * ratio));
                }

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(targetWidth, targetHeight),
                    Mode = ResizeMode.Max
                }));

                await image.SaveAsync(filePath);
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
    }
}
