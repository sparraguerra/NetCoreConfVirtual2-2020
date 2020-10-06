
using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saint.Seiya.Infrastructure
{

    public class DocumentRepository : CosmosRepositoryBase<SeiyaDocument>, IDocumentRepository
    {

        public DocumentRepository(CosmosInitializer cosmosInitializer, string collectionName) : base(cosmosInitializer, collectionName)
        {

        }

        public async Task<SeiyaDocument> AddOrUpdateCosmosAsync(string databaseName, string collectionName, SeiyaDocument entity)
        {
            var result = await this.AddOrUpdateAsync(entity);
            return result;
        }

        public async Task<bool> DeleteCosmosAsync(string documentId, string partitionKey)
        {
            var result = await this.DeleteAsync(documentId, partitionKey);
            return result;
        }

        public async Task<IEnumerable<SeiyaDocument>> GetAsync(string collectionName, string partitionKey, JObject conditions, IEnumerable<string> fields)
        {
            var sqlQuery = SqlQueryGenerator.GenerateSqlQuery(collectionName, conditions, fields);
            var result = await this.Get(partitionKey, sqlQuery, 0, 1000);

            return result;
        }

        public async Task<IEnumerable<SeiyaDocument>> GetAsync(string partitionKey, string databaseName, string collectionName, string sqlQuery)
        {
            var result = await this.Get(partitionKey, sqlQuery, 0, 1000);

            return result;
        }

        public async Task<SeiyaDocument> GetByIdCosmosAsync(string databaseName, string collectionName, string documentId, string partitionKey)
        {
            var result = await this.GetByIdAsync(documentId, partitionKey);

            return result;
        }

        public Task<SeiyaDocument> GetByIdCosmosAsync(string databaseName, string collectionName, SeiyaDocument entity)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<SeiyaDocument> GetDocumentsFromDocumentClassAsync(string documentClass, string documentClassId)
        {
            var result = this.GetAll(x => x.DocumentClass.Equals(documentClass) && x.DocumentClassId.Equals(documentClassId));
            return result;
        }
    }
}
