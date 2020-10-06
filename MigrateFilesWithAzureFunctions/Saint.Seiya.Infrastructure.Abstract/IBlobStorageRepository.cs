
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Saint.Seiya.Infrastructure.Abstract
{
    public interface IBlobStorageRepository
    {
        void SetConnectionString(string connectionString);
        Task<UploadResult> UploadBlobToContainerAsync(string containerName, string folderName, UploadRequestBase request, string guid);
        Task<GetFileResponse> DownloadBlobFromContainerAsync(string containerName, string fileId, string folderName);
        Task<GetFileResponse> DownloadBlobFromUri(Uri documentUri, ILogger logger);
        Task<string> GetBlobFilenameFromUri(Uri documentUri);
        Task<CopyResult> CopyBlob(string containerName, string tempFolderName, string destinationFolderName, string fileName, string newName, string key);
        Uri GetUriSasById(string containerName, string documentId, int minutes, string folderName);
        Task<bool> DeleteBlobAsync(string containerName, string folderName, string fileName);
        Task UpdateFileName(Uri documentUri, string newFileName);
        CloudBlockBlob GetBlobByName(string containerName, string fileName);
    }
}
