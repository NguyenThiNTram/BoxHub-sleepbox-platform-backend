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
        Task<string?> UploadFileAsync(IFormFile file);
        Task<List<string>> UploadFilesAsync(List<IFormFile> files);

        /// <summary>PDF / giấy tờ — ResourceType Raw.</summary>
        Task<string?> UploadDocumentAsync(IFormFile file);
    }
}
