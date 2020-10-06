using Saint.Ikki.Fx.Shared.Models;
using Saint.Ikki.Fx.Shared.Models.Seiya;
using System.IO;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.ApplicationServices.Abstract.Clients
{
    public interface ISeiyaApiClient
    {
        Task<ResponseModel> IndexBinaryDocumentAsync(IndexBinaryRequestModel requestModel, Stream fileStream, string fileName);
        Task<bool> DeleteDocumentAsync(DeleteRequestModel requestModel);
        Task<bool> RevertDeleteDocumentAsync(DeleteRequestModel requestModel);
        Task<UriResponseModel> GetDocumentUriAsync(string documentId, string documentClassId, string user);
        Task<bool> UpdateProcessAsync(UpdateStatusRequest requestModel);
    }
}
