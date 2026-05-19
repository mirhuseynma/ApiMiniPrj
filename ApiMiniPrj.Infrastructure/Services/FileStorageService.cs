using ApiMiniPrj.Application.Interfaces.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ApiMiniPrj.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private static readonly string[] AllowedExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ];

        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException("File is empty.", nameof(file));
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Only jpg, jpeg, png and webp files are allowed.");
            }

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var uploadFolder = Path.Combine(webRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var physicalPath = Path.Combine(uploadFolder, fileName);

            await using var stream = new FileStream(physicalPath, FileMode.CreateNew);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/{folderName}/{fileName}";
        }

        public void DeleteFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            var relativePath = filePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));
            var rootPath = Path.GetFullPath(webRootPath);

            if (!physicalPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}
