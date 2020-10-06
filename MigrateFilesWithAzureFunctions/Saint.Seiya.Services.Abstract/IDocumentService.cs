using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saint.Seiya.Services.Abstract
{
    public interface IDocumentService
    {
        Task<GetFileResponse> GetDocumentAsync(GetDocumentRequest getDocumentRequest);
        Task<DocumentResponse> GetMetadataAsync(GetDocumentRequest getDocumentRequest);
        Task<DocumentResponse> IndexDocumentAsync(IndexRequest indexRequest);
        Task<DocumentResponse> UpdateDocumentMetadataAsync(UpdateRequest updateRequest);
        Task<bool> DeleteDocumentAsync(DeleteRequest deleteRequest);
        Task<bool> RestoreDocumentAsync(RestoreRequest restoreRequest);
        Task<GetUriResponse> GetDocumentSasUriAsync(GetDocumentRequest getDocumentRequest); 
        Task<DocumentResponse> IndexDocumentWithBinaryAsync(IndexDocumentWithBinaryrequest indexWithBinaryRequest);
        Task<IEnumerable<ProcessResponse>> GetProcesses();
        TracesResponse GetTraces(TracesRequest request);
        Task<TraceProcessResponse> LaunchProcessAsync(LaunchProcessRequest request);
        Task<bool> UpdateProcess(UpdateStatusRequest updateRequest);
    }
}
