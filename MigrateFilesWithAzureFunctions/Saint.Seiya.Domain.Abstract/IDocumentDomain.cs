using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saint.Seiya.Domain.Abstract
{
    public interface IDocumentDomain
    {
        Task<GetFileResponse> GetDocumentAsync(GetDocumentRequest getDocumentRequest);
        Task<DocumentResponse> GetMetadataAsync(GetDocumentRequest getDocumentRequest);
        Task<DocumentResponse> IndexDocumentAsync(IndexRequest indexRequest);
        Task<DocumentResponse> UpdateDocumentMetadataAsync(UpdateRequest updateRequest);
        Task<TraceProcessResponse> UpdateProcess(UpdateStatusRequest updateRequest);
        Task<bool> DeleteDocumentAsync(DeleteRequest deleteRequest);
        Task<bool> RestoreDocumentAsync(RestoreRequest restoreRequest);
        Task<GetUriResponse> GetDocumentSasUriAsync(GetDocumentRequest getDocumentRequest); 
        Task<DocumentResponse> IndexDocumentWithBinaryAsync(IndexDocumentWithBinaryrequest indexWithBinaryRequest);
        Task<IEnumerable<ProcessResponse>> GetProcesses();
        Task<TraceProcessResponse> LaunchProcessAsync(LaunchProcessRequest request);
        TracesResponse GetTraces(TracesRequest request);
    }
}
