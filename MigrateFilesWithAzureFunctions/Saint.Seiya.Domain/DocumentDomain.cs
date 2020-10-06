using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saint.Seiya.Domain.Abstract;
using Saint.Seiya.Domain.Helpers;
using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Config;
using Saint.Seiya.Shared.Models.Dto;
using Saint.Seiya.Shared.Models.Enums;
using Saint.Seiya.Shared.Models.Exceptions;
using Saint.Seiya.Shared.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Saint.Seiya.Domain
{
    public class DocumentDomain : IDocumentDomain
    {
        private readonly IDocumentRepository documentRepository;
        private readonly ITraceRepository traceRepository;
        private readonly IProcessRepository processRepository;
        private readonly IBlobStorageRepository blobStorageRepository;
        private readonly HttpClient client;
        private readonly IMapper mapper;
        private readonly int minutesUntilSasExpiration; 
        private readonly CosmosDBConfig cosmosConfig;
        private readonly BlobStorageConfig blobStorageConfig;
        private readonly AppSettings appSettings;

        public DocumentDomain(IDocumentRepository documentRepository, IBlobStorageRepository blobStorageRepository, ITraceRepository traceRepository, 
            IProcessRepository processRepository,IMapper mapper, IOptions<CosmosDBConfig> cosmosConfig,
            IOptions<BlobStorageConfig> blobStorageConfig, IOptions<AppSettings> appSettings, HttpClient client)
        {
            this.documentRepository = documentRepository;
            this.blobStorageRepository = blobStorageRepository;
            this.traceRepository = traceRepository;
            this.processRepository = processRepository;
            this.mapper = mapper;
            this.client = client;
            this.appSettings = appSettings.Value;
            this.cosmosConfig = cosmosConfig.Value;
            this.blobStorageConfig = blobStorageConfig.Value;
            minutesUntilSasExpiration = this.blobStorageConfig.MinutesSasExpire;
            blobStorageRepository.SetConnectionString(this.blobStorageConfig.ConnectionString);
        }

        /// <summary>
        /// DeleteDocumentAsync
        /// </summary>
        /// <param name="deleteRequest"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDocumentAsync(DeleteRequest deleteRequest)
        {
            ValidateIsNotNull(deleteRequest, nameof(DeleteRequest));

            var document = await GetByIdAsync(deleteRequest);
            if (string.IsNullOrWhiteSpace(document?.Id))
            {

                throw new SeiyaValidationException("Document Id can not be null or empty");
            }
            if (string.IsNullOrWhiteSpace(document?.DocumentClassId))
            {
                throw new SeiyaValidationException("Document Class Id can not be null or empty");
            }

            document.Delete(deleteRequest.User);
            await documentRepository.AddOrUpdateCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection, document);

            return true;
        }

        /// <summary>
        /// RestoreDocumentAsync 
        /// </summary>
        /// <param name="restoreRequest"></param>
        /// <returns></returns>
        public async Task<bool> RestoreDocumentAsync(RestoreRequest restoreRequest)
        {
            ValidateIsNotNull(restoreRequest, nameof(RestoreRequest));
            var document = await GetRestoreByIdAsync(restoreRequest);
            document.Restore(restoreRequest.User);
            await documentRepository.AddOrUpdateCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection, document);

            return true;
        }


        /// <summary>
        /// GetDocumentAsync
        /// </summary>
        /// <param name="getDocumentRequest"></param>
        /// <returns></returns>
        public async Task<GetFileResponse> GetDocumentAsync(GetDocumentRequest getDocumentRequest)
        {
            var document = await GetMetadataAsync(getDocumentRequest);
            if (string.IsNullOrWhiteSpace(document?.Id))
            {

                throw new SeiyaValidationException("Document Id can not be null or empty");
            }
            if (string.IsNullOrWhiteSpace(document?.DocumentClassId))
            {
                throw new SeiyaValidationException("Document Class Id can not be null or empty");
            }
            var uriAttachment = document.GetUri();
            var documentId = uriAttachment.Segments.Last();
            var folder = $"{document.DocumentClass}-{document.DocumentClassId}";

            var result = await blobStorageRepository.DownloadBlobFromContainerAsync(blobStorageConfig.Container, documentId, folder);
            return result;
        }

        /// <summary>
        /// GetDocumentSasUriAsync
        /// </summary>
        /// <param name="getDocumentRequest"></param>
        /// <returns></returns>
        public async Task<GetUriResponse> GetDocumentSasUriAsync(GetDocumentRequest getDocumentRequest)
        {
            var result = new GetUriResponse()
            {
                DocumentId = getDocumentRequest.Id
            };

            var document = await GetMetadataAsync(getDocumentRequest);
            if (string.IsNullOrWhiteSpace(document?.Id))
            {

                throw new SeiyaValidationException("Document Id can not be null or empty");
            }
            if (string.IsNullOrWhiteSpace(document?.DocumentClassId))
            {
                throw new SeiyaValidationException("Document Class Id can not be null or empty");
            }
            var uriAttachment = document.GetUri();
            var documentId = uriAttachment.Segments.Last();
            var folder = $"{document.DocumentClass}-{document.DocumentClassId}";

            var finalUriWithSas = blobStorageRepository.GetUriSasById(blobStorageConfig.Container, documentId, minutesUntilSasExpiration, folder);
            result.Uri = finalUriWithSas;

            return new GetUriResponse()
            {
                DocumentId = getDocumentRequest.Id,
                Uri = finalUriWithSas
            };
        }

        /// <summary>
        /// GetMetadataAsync
        /// </summary>
        /// <param name="getDocumentRequest"></param>
        /// <returns></returns>
        public async Task<DocumentResponse> GetMetadataAsync(GetDocumentRequest getDocumentRequest)
        {
            ValidateIsNotNull(getDocumentRequest, nameof(GetDocumentRequest));
            var document = await GetByIdAsync(getDocumentRequest);
            if (string.IsNullOrWhiteSpace(document?.Id))
            {
                throw new SeiyaValidationException("Document Id can not be null or empty");
            }
            if (string.IsNullOrWhiteSpace(document?.DocumentClassId))
            {
                throw new SeiyaValidationException("Document Class Id can not be null or empty");
            }
            var documentUri = document.GetUri();

            var docName64 = await blobStorageRepository.GetBlobFilenameFromUri(documentUri);
            var docName = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(docName64));
            document.FileName = docName;
            var result = mapper.Map<SeiyaDocument, DocumentResponse>(document);
            return result;
        }

        /// <summary>
        /// IndexDocumentAsync
        /// </summary>
        /// <param name="indexRequest"></param>
        /// <returns></returns>
        public async Task<DocumentResponse> IndexDocumentAsync(IndexRequest indexRequest)
        {
            ValidateIsNotNull(indexRequest, nameof(IndexRequest));
            var document = mapper.Map<IndexRequest, SeiyaDocument>(indexRequest);
            var documentId = string.Empty;
            Uri documentUri = null;
            string partitionKey = string.Empty;

            try
            {
                partitionKey = IdHelper.PartitionKeyGenerator(cosmosConfig.AppId, document.DocumentClassId);
                document.PartitionKey = partitionKey;
                document.Id = IdHelper.GuidGenerator();
                var copyResult = await ExtractDocumentFromTempStorageAsync(indexRequest, document);
                document.FileName = copyResult.OriginalFileName;
                document.ContentType = copyResult.ContentType;
                document.Size = copyResult.Size;
                document.AbsoluteUri = copyResult.Uri.AbsoluteUri;
                document.BlobName = copyResult.BlobName;
                document.FolderName = copyResult.FolderName;
                document.Container = copyResult.Container;

                var indexedDoc = await documentRepository.AddOrUpdateCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection, document);

                await blobStorageRepository.DeleteBlobAsync(blobStorageConfig.Container, blobStorageConfig.Temp, indexRequest.FileName);


                return mapper.Map<SeiyaDocument, DocumentResponse>(indexedDoc);
            }
            catch (Exception)
            {
                await CleanupFailedData(documentId, documentUri, partitionKey);
                throw;
            }
        }

        /// <summary>
        /// IndexDocumentWithBinaryAsync
        /// </summary>
        /// <param name="indexWithBinaryRequest"></param>
        /// <returns></returns>
        public async Task<DocumentResponse> IndexDocumentWithBinaryAsync(IndexDocumentWithBinaryrequest indexWithBinaryRequest)
        {
            var documentId = string.Empty;
            Uri documentUri = null;
            string partitionKey = string.Empty;

            try
            {
                var indexRequestWithBinaryDocument = JsonConvert.DeserializeObject<IndexRequestWithBinary>(indexWithBinaryRequest.Metadata);
                var uploadRequest = new UploadRequest()
                {
                    FileStream = indexWithBinaryRequest.File.OpenReadStream(),
                    ContentType = indexWithBinaryRequest.File.ContentType,
                    DocumentClass = indexRequestWithBinaryDocument.DocumentClass,
                    DocumentClassId = indexRequestWithBinaryDocument.DocumentClassId,
                    Extension = Path.GetExtension(indexWithBinaryRequest.File.FileName),
                    OriginalFileName = indexWithBinaryRequest.File.FileName
                };
                var document = mapper.Map<IndexRequestWithBinary, SeiyaDocument>(indexRequestWithBinaryDocument);
                partitionKey = IdHelper.PartitionKeyGenerator(cosmosConfig.AppId, document.DocumentClassId);
                document.PartitionKey = partitionKey;
                document.Id = IdHelper.GuidGenerator();
                document.FileName = uploadRequest.OriginalFileName;
                document.ContentType = uploadRequest.ContentType;
                document.Size = uploadRequest.FileStream.Length;

                UploadResult uploadResult = await UploadDocumentAsync(uploadRequest, document.Id);
                document.AbsoluteUri = uploadResult.AbsoluteUri;
                document.BlobName = uploadResult.BlobName;
                document.Container = uploadResult.Container;
                document.FolderName = uploadResult.FolderName;

                var indexedDoc = await documentRepository.AddOrUpdateCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection, document);

                var result = mapper.Map<SeiyaDocument, DocumentResponse>(indexedDoc);

                return result;
            }
            catch (Exception)
            {
                await CleanupFailedData(documentId, documentUri, partitionKey);
                throw;
            }
        }

        /// <summary>
        /// UpdateDocumentMetadataAsync
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        public async Task<DocumentResponse> UpdateDocumentMetadataAsync(UpdateRequest updateRequest)
        {
            GetDocumentRequest documentRequest = new GetDocumentRequest();
            documentRequest.Id = updateRequest.Id;
            documentRequest.DocumentClassId = updateRequest.DocumentClassId;

            var document = await GetByIdAsync(documentRequest);
            if (updateRequest.Properties != null && updateRequest.Properties?.Count > 0)
            {
                var properties = JObject.FromObject(document.Properties);
                properties.Merge(updateRequest.Properties, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                document.Properties = properties;
            }

            if (!string.IsNullOrWhiteSpace(updateRequest.DocumentType))
            {
                document.DocumentType = updateRequest.DocumentType;
            }

            document.LastModifiedUser = updateRequest.User;
            document.Modified = DateTime.UtcNow;

            var updatedDoc = await documentRepository.AddOrUpdateCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection, document);
            await UpdateBlobFileNameAsync(updateRequest);

            return mapper.Map<SeiyaDocument, DocumentResponse>(updatedDoc);
        }

        /// <summary>
        /// UploadRequest
        /// </summary>
        /// <param name="uploadRequest"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<UploadResult> UploadDocumentAsync(UploadRequest uploadRequest, string guid)
        {
            var folderName = $"{uploadRequest.DocumentClass}-{uploadRequest.DocumentClassId}";
            return await blobStorageRepository.UploadBlobToContainerAsync(blobStorageConfig.Container, folderName, uploadRequest, guid);
        }


        /// <summary>
        /// CopyDocumentAsync
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="originFolderName"></param>
        /// <param name="destinationFolderName"></param>
        /// <returns></returns>
        private async Task<CopyResult> CopyDocumentAsync(string fileName, string originFolderName, string destinationFolderName, string newName, string key)
        {
            if (originFolderName.Equals(destinationFolderName))
            {
                throw new SeiyaConflictException("Destination folder can not be same as origin folder");
            }
            return await blobStorageRepository.CopyBlob(blobStorageConfig.Container, originFolderName, destinationFolderName, fileName, newName, key);
        }

        /// <summary>
        /// ExtractDocumentFromTempStorageAsync
        /// </summary>
        /// <param name="documentDto"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private async Task<CopyResult> ExtractDocumentFromTempStorageAsync(IndexRequest documentDto, SeiyaDocument document)
        {
            var folderName = $"{document.DocumentClass}-{document.DocumentClassId}";
            var pos = document.FileName.IndexOf(".", StringComparison.CurrentCulture);
            var extension = document.FileName.Substring(pos);
            var newName = document.Id + extension;
            var key = IdHelper.GetSearchKey(document);
            return await CopyDocumentAsync(documentDto.FileName, blobStorageConfig.Temp, folderName, newName, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentRequest"></param>
        /// <returns></returns>
        private async Task<SeiyaDocument> GetByIdAsync(GetDocumentRequest documentRequest)
        {
            SeiyaDocument document = await documentRepository.GetByIdCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection,
                                    documentRequest.Id, IdHelper.PartitionKeyGenerator(cosmosConfig.AppId, documentRequest.DocumentClassId));

            if (document?.IsDeleted == true)
            {
                throw new SeiyaDataNotFoundException($"Document {documentRequest.Id} not found");
            }
            return document;
        }

        /// <summary>
        /// GetRestoreByIdAsync
        /// </summary>
        /// <param name="documentRequest"></param>
        /// <returns></returns>
        private async Task<SeiyaDocument> GetRestoreByIdAsync(GetDocumentRequest documentRequest)
        {
            var document = await documentRepository.GetByIdCosmosAsync(cosmosConfig.Database, cosmosConfig.Collection,
                            documentRequest.Id, IdHelper.PartitionKeyGenerator(cosmosConfig.AppId, documentRequest.DocumentClassId));
            if (!document.IsDeleted)
            {
                throw new SeiyaDataNotFoundException($"Document {documentRequest.Id} not found");
            }
            return document;
        }

        /// <summary>
        /// ValidateIsNotNull
        /// </summary>
        /// <param name="objectToTest"></param>
        /// <param name="className"></param>
        private void ValidateIsNotNull(object objectToTest, string className)
        {
            if (objectToTest == null)
            {
                throw new SeiyaValidationException($"{className} can not be null");
            }
        }

        /// <summary>
        /// CleanupFailedData
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="documentUri"></param>
        /// <returns></returns>
        private async Task CleanupFailedData(string documentId, Uri documentUri, string partitionKey)
        {
            if (!string.IsNullOrWhiteSpace(documentId))
            {
                await documentRepository.DeleteCosmosAsync(documentId, partitionKey);
            }

            if (documentUri != null)
            {
                var segments = documentUri.Segments;

                var fileName = segments[segments.Length - 1];
                var folderName = segments[segments.Length - 2].Trim('/');

                await blobStorageRepository.DeleteBlobAsync(blobStorageConfig.Container, folderName, fileName);
            }
        }

        /// <summary>
        /// UpdateBlobFileNameAsync
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        private async Task UpdateBlobFileNameAsync(UpdateRequest updateRequest)
        {
            if (updateRequest.Properties.ContainsKey("FileName"))
            {
                var documentUri = await GetDocumentUriAsync(new GetDocumentRequest
                {
                    Id = updateRequest.Id,
                    User = updateRequest.User
                });
                await blobStorageRepository.UpdateFileName(documentUri, updateRequest.Properties.Value<string>("FileName"));
            }
        }

        /// <summary>
        /// GetDocumentUriAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<Uri> GetDocumentUriAsync(GetDocumentRequest request)
        {
            ValidateIsNotNull(request, nameof(GetDocumentRequest));
            SeiyaDocument gadaDocument = await GetByIdAsync(request);
            return gadaDocument.GetUri();
        }

        public async Task<IEnumerable<ProcessResponse>> GetProcesses()
        {
            return processRepository.GetAll();
        }

        public async Task<TraceProcessResponse> LaunchProcessAsync(LaunchProcessRequest request)
        {
            var process = processRepository.GetAll(c=> c.Id == request.ProcessType.ToString()).FirstOrDefault();

            if (process is null && request.ProcessType != 99)
            {
                throw new InvalidOperationException("Process not found");
            }

            var data = new ModelFunctionRequest
            {
                User = request.User,
                Type = request.ProcessType,
                UniqueId = Guid.NewGuid().ToString()
            };

            var myContent = JsonConvert.SerializeObject(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(appSettings.SaintIkkiConfig.Endpoint, byteContent);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error azure functions call");
            }

            TraceProcessResponse result = null;

            if (ProcessingOrchestator(request.ProcessType))
            {
                throw new InvalidOperationException($"Process '{process?.Name}' is processing now. Try when it finalize");
            }

            result = new TraceProcessResponse()
            {
                Id = Guid.NewGuid().ToString(),
                Name = process?.Name,
                ProcessType = process?.Id,
                PartitionKey = process?.Id,
                UserMail = request.User,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Status = ProcessStatus.InExecution.ToString(),
                UniqueId = data.UniqueId
            };

            await traceRepository.AddOrUpdateAsync(result);


            return result;
        }

        private bool ProcessingOrchestator(int processId)
        {
            var processes = traceRepository.GetAll(x => x.ProcessType == processId.ToString() && x.Status == ProcessStatus.InExecution.ToString());

            return processes.Any();
        }

        public async Task<TraceProcessResponse> UpdateProcess(UpdateStatusRequest updateRequest)
        {

            var process = traceRepository.GetAll(c => c.ProcessType == updateRequest.ProcessType.ToString() && c.UniqueId == updateRequest.UniqueId).FirstOrDefault();

            if (process is null)
            {
                throw new InvalidOperationException("Process not found");
            }

            var result = new TraceProcessResponse()
            {
                Id = process.Id,
                Name = process.Name,
                ProcessType = process.ProcessType,
                UserMail = process.UserMail,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Status = updateRequest.Status.ToString(),
                UniqueId = process.UniqueId,
                PartitionKey = process.PartitionKey
            };

            var response = await traceRepository.AddOrUpdateAsync(result);

            return response;
        }

        public TracesResponse GetTraces(TracesRequest request)
        {
            var result = new TracesResponse();

            Expression<Func<TraceProcessResponse, dynamic>> order = null;

            order = request.OrderBy switch
            {
                "name" => o => o.Name,
                "status" => o => o.Status,
                _ => o => o.Date,
            };

            if (request.Filter == null)
            {
                result.TotalCount = traceRepository.CountAll();
                result.ProcessList = traceRepository.GetAll(s => true, order, request.Descending, request.ElementsPerPage * (request.Page - 1), request.ElementsPerPage);
            }
            else
            {
                result.TotalCount = traceRepository.CountAll(s => s.Status.Equals(request.Filter.ToString()));
                result.ProcessList = traceRepository.GetAll(s => s.Status.Equals(request.Filter.ToString()), order,
                    request.Descending, request.ElementsPerPage * (request.Page - 1), request.ElementsPerPage);
            }

            result.CurrentPage = request.Page;

            return result;
        }
    }
}
