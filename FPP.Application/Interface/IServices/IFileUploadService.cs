using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface IFileUploadService
    {
        Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadImageAsync(IFormFile file, string folder = "security-logs");
        bool DeleteImage(string filePath);
        bool IsValidImage(IFormFile file);
    }
}
