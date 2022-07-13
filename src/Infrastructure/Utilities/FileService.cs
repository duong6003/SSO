using Amazon.S3;
using Core.Common.Interfaces;
using Core.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Utilities
{
    public interface IFileService : IScopedService
    {
        Task<string?> UploadFileAsync(IFormFile file, string actionName);
        Task<(string? Path, string? ErrorMessage)> UpdateFileAsync(IFormFile file, string actionName, string? oldPath);
        Task<bool> DeleteFileAsync(string? oldPath);
        string? GetFullPath(string? path);
        string? GetPath(string? path);
    }
    public class FileService : IFileService
    {
        private readonly IAmazonS3Utility AmazonS3Utility;
        private readonly IConfiguration Configuration;

        public FileService(IAmazonS3Utility amazonS3Utility, IConfiguration configuration)
        {
            AmazonS3Utility = amazonS3Utility;
            Configuration = configuration;
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string actionName)
        {
            string? fileName = new Random().Next() + "_" + Regex.Replace(file.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
            (string? filePath, string? error) = await AmazonS3Utility.SaveFileAmazonS3Async(file, Configuration["AmazonS3:BucketName"], actionName + "/" + fileName, S3CannedACL.PublicRead);
            if (error is not null) Log.Error(error);
            return filePath;
        }
        public async Task<(string? Path, string? ErrorMessage)> UpdateFileAsync(IFormFile file, string actionName, string? oldPath)
        {
            string? fileName = actionName + "/" + new Random().Next() + "_" + Regex.Replace(file.FileName.Trim(), @"[^a-zA-Z0-9-_.]", "");
            (string? filePath, string? error) = await AmazonS3Utility.UpdateResourceAsync(file, Configuration["AmazonS3:BucketName"], S3CannedACL.PublicRead, fileName, oldPath);
            if (error is not null) return (null, error);
            return (filePath, null);
        }
        public async Task<bool> DeleteFileAsync(string? oldPath)
        {
            (bool success, string? error) = await AmazonS3Utility.DeleteResourceAsync(Configuration["AmazonS3:BucketName"], oldPath);
            if (error is not null)
            {
                Log.Error(error);
                return false;
            }
            return success;
        }

        public string? GetFullPath(string? path)
        {
            return AmazonS3Utility.GetPublicUrl(Configuration["AmazonS3:PublicDomain"], Configuration["AmazonS3:BucketName"], path);
        }
        public string? GetPath(string? path)
        {
            return AmazonS3Utility.GetPath(path);
        }
    }

}
