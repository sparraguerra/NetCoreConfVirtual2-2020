using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models;

namespace Saint.Seiya.Infrastructure
{
    public class ProcessRepository : CosmosRepositoryBase<ProcessResponse>, IProcessRepository
    {
        public ProcessRepository(CosmosInitializer cosmosInitializer, string collectionName) : base(cosmosInitializer, collectionName)
        {

        }
    }
}
