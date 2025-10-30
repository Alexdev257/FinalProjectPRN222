using FPP.Application.Interface.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Hosting;

namespace FPP.Infrastructure.Implements.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IHostEnvironment _environment; // đổi IWebHostEnvironment -> IHostEnvironment
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public FileUploadService(IHostEnvironment environment) // đổi tham số luôn
        {
            _environment = environment;
        }

        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadImageAsync(
             IFormFile file, string folder = "security-logs")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, null, "No file uploaded");

                if (file.Length > _maxFileSize)
                    return (false, null, $"File size exceeds {_maxFileSize / 1024 / 1024}MB limit");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return (false, null, $"Invalid file type. Allowed: {string.Join(", ", _allowedExtensions)}");

                // Dùng ContentRootPath thay vì WebRootPath
                var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", folder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var relativePath = $"/uploads/{folder}/{fileName}";
                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Upload failed: {ex.Message}");
            }
        }

        public bool DeleteImage(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_environment.ContentRootPath, filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }
    }
}
