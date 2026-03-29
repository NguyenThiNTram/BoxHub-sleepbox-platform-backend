using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface ICloudinaryService
    {
        Task<string?> UploadImageAsync(IFormFile file);
        Task<List<string>> UploadImagesAsync(List<IFormFile> files);

        /// <summary>PDF / giấy tờ — ResourceType Raw.</summary>
        Task<string?> UploadFileAsync(IFormFile file);

        /// <summary>PDF / giấy tờ — ResourceType Raw (nhiều file).</summary>
        Task<List<string>> UploadFilesAsync(List<IFormFile> files);
    }
}
