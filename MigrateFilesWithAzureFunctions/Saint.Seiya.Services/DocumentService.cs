using Saint.Seiya.Domain.Abstract;
using Saint.Seiya.Services.Abstract;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saint.Seiya.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentDomain documentDomain;

        public DocumentService(IDocumentDomain documentDomain)
        {
            this.documentDomain = documentDomain;
        }
        public async Task<bool> DeleteDocumentAsync(DeleteRequest deleteRequest)
        {
            return await documentDomain.DeleteDocumentAsync(deleteRequest);
        }

        public async Task<GetFileResponse> GetDocumentAsync(GetDocumentRequest getDocumentRequest)
        {
            return await documentDomain.GetDocumentAsync(getDocumentRequest);
        }

        public async Task<GetUriResponse> GetDocumentSasUriAsync(GetDocumentRequest getDocumentRequest)
        {
            return await documentDomain.GetDocumentSasUriAsync(getDocumentRequest);
        }

        public async Task<DocumentResponse> GetMetadataAsync(GetDocumentRequest getDocumentRequest)
        {
            return await documentDomain.GetMetadataAsync(getDocumentRequest);
        }

        public async Task<IEnumerable<ProcessResponse>> GetProcesses()
        {
            return await documentDomain.GetProcesses();
        }

        public TracesResponse GetTraces(TracesRequest request)
        {
            return documentDomain.GetTraces(request);
        }

        public async Task<DocumentResponse> IndexDocumentAsync(IndexRequest indexRequest)
        {
            return await documentDomain.IndexDocumentAsync(indexRequest);
        } 
       
        public async Task<DocumentResponse> IndexDocumentWithBinaryAsync(IndexDocumentWithBinaryrequest indexWithBinaryRequest)
        {
            return await documentDomain.IndexDocumentWithBinaryAsync(indexWithBinaryRequest);
        }

        public async Task<TraceProcessResponse> LaunchProcessAsync(LaunchProcessRequest request)
        {
            return await documentDomain.LaunchProcessAsync(request);
        }

        public async Task<bool> RestoreDocumentAsync(RestoreRequest restoreRequest)
        {
            return await documentDomain.RestoreDocumentAsync(restoreRequest);
        }

        public async Task<DocumentResponse> UpdateDocumentMetadataAsync(UpdateRequest updateRequest)
        {
            return await documentDomain.UpdateDocumentMetadataAsync(updateRequest);
        }

        public async Task<bool> UpdateProcess(UpdateStatusRequest updateRequest)
        {
            return await documentDomain.UpdateProcess(updateRequest) != null;
        }
    }
}
