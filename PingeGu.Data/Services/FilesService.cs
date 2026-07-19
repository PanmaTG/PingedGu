using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using PingedGu.Data.Helpers.Enums;
using PingedGu.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PingedGu.Data.Services
{
    public class FilesService : IFilesService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public FilesService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType)
        {
            string containerName = imageFileType switch
            {
                ImageFileType.PostImage => "posts",
                ImageFileType.StoryImage => "stories",
                ImageFileType.ProfilePicture => "profilepicture",
                ImageFileType.CoverImage => "covers",
                _ => throw new ArgumentException("Invalid file type")
            };

            if (file == null || file.Length == 0)
                return "";

            // Check if container exist
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Generate file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // Upload file to Azure Blob Storage
            using(var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            return blobClient.Uri.ToString();
        }
    }
}
