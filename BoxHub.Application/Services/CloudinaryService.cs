using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Helpers.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudSettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.CloudKey,
                config.Value.CloudSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is invalid.");

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "BoxHub_Uploads",
                };

                try
                {
                    var result = await _cloudinary.UploadAsync(uploadParams);
                    return result.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to upload image to Cloudinary.", ex);
                }
            }
        }

        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                var url = await UploadFileAsync(file);
                if (url != null)
                {
                    urls.Add(url);
                }
            }
            return urls;
        }

        public async Task<string?> UploadDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is invalid.");

            await using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "BoxHub_HostDocuments",
            };

            try
            {
                var result = await _cloudinary.UploadAsync(uploadParams);
                return result.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload document to Cloudinary.", ex);
            }
        }
    }
}
