using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Extensions.Logging;
using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using Saint.Seiya.Shared.Models.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Saint.Seiya.Infrastructure
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private string connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="folderName"></param>
        /// <param name="request"></param>
        /// <param name="guid"></param> 
        /// <returns></returns>
        public async Task<UploadResult> UploadBlobToContainerAsync(string containerName, string folderName, UploadRequestBase request, string guid)
        {
            var client = GetClient();
            var container = client.GetContainerReference(containerName);
            var folder = container.GetDirectoryReference(folderName);
            var blobName = $"{guid}{request.Extension}";
            try
            {
                var blockBlob = folder.GetBlockBlobReference(blobName);
                blockBlob.Properties.ContentType = request.ContentType;
                blockBlob.Metadata.Add("originalFileName", EncodeFileNameToBase64(request.OriginalFileName));               

                TransferManager.Configurations.ParallelOperations = ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 8;
                ServicePointManager.Expect100Continue = false;

                await TransferManager.UploadAsync(request.FileStream, blockBlob);

                return new UploadResult
                {
                    AbsoluteUri = blockBlob.Uri.AbsoluteUri,
                    Container = containerName,
                    BlobName = blobName,
                    FolderName = folderName
                };

            }
            catch (StorageException ex)
            {
                throw new SeiyaDataAccessException($"Error while uploading blob to container {containerName}.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<GetFileResponse> DownloadBlobFromContainerAsync(string containerName, string fileId, string folderName)
        {
            try
            {
                var result = new GetFileResponse();
                var client = GetClient();
                var container = client.GetContainerReference(containerName);
                var folder = container.GetDirectoryReference(folderName);

                var blockBlob = folder.GetBlockBlobReference(fileId);
                var fileStream = new MemoryStream();

                await blockBlob.DownloadToStreamAsync(fileStream);

                result.ContentType = blockBlob.Properties.ContentType;
                result.File = fileStream;
                result.FileName = DecodeBase64FileName(blockBlob.Metadata["originalFileName"]);

                return result;
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                throw new SeiyaDataNotFoundException(fileId, "File not found", ex);
            }
            catch (StorageException ex)
            {
                throw new SeiyaDataAccessException(fileId, "Error while querying storage", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentUri"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobFromUri(Uri documentUri, ILogger logger)
        {
            var client = GetClient();
            var cloudBlob = new CloudBlockBlob(documentUri, client);

            MemoryStream documentStream = new MemoryStream();
            await cloudBlob.DownloadToStreamAsync(documentStream);

            return documentStream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentUri"></param>
        /// <returns></returns>
        public async Task<string> GetBlobFilenameFromUri(Uri documentUri)
        {
            var client = GetClient();
            var cloudBlob = new CloudBlockBlob(documentUri, client);
            await cloudBlob.FetchAttributesAsync();
            return cloudBlob.Metadata["originalFileName"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentUri"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<GetFileResponse> DownloadBlobFromUri(Uri documentUri, ILogger logger)
        {
            try
            {
                var result = new GetFileResponse();
                var client = GetClient();
                var cloudBlockBlob = new CloudBlockBlob(documentUri, client);

                var fileStream = new MemoryStream();

                await cloudBlockBlob.DownloadToStreamAsync(fileStream);

                result.ContentType = cloudBlockBlob.Properties.ContentType;
                result.File = fileStream;
                result.FileName = cloudBlockBlob.Uri.Segments.Last();

                return result;
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                throw new SeiyaDataNotFoundException(documentUri.Segments.Last(), "File not found", ex);
            }
            catch (StorageException ex)
            {
                throw new SeiyaDataAccessException(documentUri.Segments.Last(), "Error while querying storage", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="documentId"></param>
        /// <param name="minutes"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public Uri GetUriSasById(string containerName, string documentId, int minutes, string folderName)
        {
            try
            {
                var client = GetClient();
                var container = client.GetContainerReference(containerName);
                var folder = container.GetDirectoryReference(folderName);

                var blockBlob = folder.GetBlockBlobReference(documentId);

                var sasConstraints = new SharedAccessBlobPolicy
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(minutes),
                    Permissions = SharedAccessBlobPermissions.Read
                };

                var sasContainerToken = blockBlob.GetSharedAccessSignature(sasConstraints);

                var uriWithSas = new Uri($"{blockBlob.Uri}{sasContainerToken}");

                return uriWithSas;
            }
            catch (Exception ex)
            {
                throw new SeiyaDataAccessException(documentId, $"Error while getting blob uri with sas ({documentId}) from container {containerName}.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="tempFolderName"></param>
        /// <param name="destinationFolderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<CopyResult> CopyBlob(string containerName, string tempFolderName, string destinationFolderName, string fileName, string newName, string key)
        {
            try
            {
                var client = GetClient();
                var container = client.GetContainerReference(containerName);
                var folder = container.GetDirectoryReference(tempFolderName);

                var detinationFolder = container.GetDirectoryReference(destinationFolderName);

                var newBlob = detinationFolder.GetBlockBlobReference(newName);

                var copyMethods = new CopyMethod();

                await TransferManager.CopyAsync(folder.GetBlockBlobReference(fileName), newBlob, copyMethods);

                if (newBlob != null && !newBlob.Metadata.Any(m=>m.Key.Equals("searchId")))
                {
                   
                        newBlob.Metadata.Add("searchId", key);
                    

                }
                    if (newBlob.Metadata["searchId"] != key)
                    {
                        newBlob.Metadata["searchId"] = key;
                    }
               
                await newBlob.SetMetadataAsync();

                return new CopyResult
                {
                    Uri = newBlob.Uri,
                    ContentType = newBlob.Properties.ContentType,
                    OriginalFileName = DecodeBase64FileName(newBlob.Metadata["originalFileName"]),
                    Size = newBlob.Properties.Length,
                    FolderName = destinationFolderName,
                    Container = newBlob.Container.Name,
                    AbsoluteUri = newBlob.Uri.AbsoluteUri,
                    BlobName = newName,
                };
            }
            catch (InvalidOperationException ex) when (ex.InnerException is StorageException)
            {
                if ((ex.InnerException as StorageException).RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    throw new SeiyaDataNotFoundException(fileName, ex.Message, ex);
                }

                throw new SeiyaDataAccessException(fileName, $"Error while moving blob ({fileName}) to container {containerName} and folder {destinationFolderName}.", ex);
            }
            catch (TransferSkippedException ex)
            {
                throw new SeiyaConflictException($"The file with name {fileName} was already indexed", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBlobAsync(string containerName, string folderName, string fileName)
        {
            try
            {
                var client = GetClient();
                var container = client.GetContainerReference(containerName);
                var folder = container.GetDirectoryReference(folderName);
                var blob = folder.GetBlobReference(fileName);
                return await blob.DeleteIfExistsAsync();
            }
            catch (InvalidOperationException ex) when (ex.InnerException is StorageException)
            {
                if ((ex.InnerException as StorageException).RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    throw new SeiyaDataNotFoundException(fileName, ex.Message, ex);
                }
                throw new SeiyaDataAccessException(fileName, $"Error while deleting blob ({fileName}) from container {containerName} and folder {folderName}.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentUri"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public async Task UpdateFileName(Uri documentUri, string newFileName)
        {
            var client = GetClient();
            var cloudBlob = new CloudBlockBlob(documentUri, client);
            cloudBlob.FetchAttributes();

            if (cloudBlob.Metadata.ContainsKey("originalFileName"))
            {
                cloudBlob.Metadata["originalFileName"] = EncodeFileNameToBase64(newFileName);
                await cloudBlob.SetMetadataAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private CloudBlobClient GetClient()
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string EncodeFileNameToBase64(string fileName)
        {
            var originBytes = Encoding.UTF8.GetBytes(fileName);
            return Convert.ToBase64String(originBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        private string DecodeBase64FileName(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

        public CloudBlockBlob GetBlobByName(string containerName, string fileName)
        {
            var client = GetClient();
            var container = client.GetContainerReference(containerName);

            return container.GetBlockBlobReference(fileName);
        }

    }
}
