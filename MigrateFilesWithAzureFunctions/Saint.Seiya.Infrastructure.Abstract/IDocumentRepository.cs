using Newtonsoft.Json.Linq;
using Saint.Seiya.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saint.Seiya.Infrastructure.Abstract
{
    public interface IDocumentRepository : ICosmosRepositoryBase<SeiyaDocument>
    {
        IEnumerable<SeiyaDocument> GetDocumentsFromDocumentClassAsync(string documentClass, string documentClassId);
        Task<SeiyaDocument> GetByIdCosmosAsync(string databaseName, string collectionName, string documentId, string partitionKey);
        Task<SeiyaDocument> GetByIdCosmosAsync(string databaseName, string collectionName, SeiyaDocument entity);
        Task<IEnumerable<SeiyaDocument>> GetAsync(string partitionKey, string databaseName, string collectionName, string sqlQuery);
        Task<IEnumerable<SeiyaDocument>> GetAsync(string collectionName, string partitionKey, JObject conditions, IEnumerable<string> fields);
        Task<bool> DeleteCosmosAsync(string documentId, string partitionKey);
        Task<SeiyaDocument> AddOrUpdateCosmosAsync(string databaseName, string collectionName, SeiyaDocument entity);
    }
}
