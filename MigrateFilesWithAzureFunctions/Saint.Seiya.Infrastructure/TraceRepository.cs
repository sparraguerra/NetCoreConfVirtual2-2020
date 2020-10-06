using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models.Dto;

namespace Saint.Seiya.Infrastructure
{
    public class TraceRepository : CosmosRepositoryBase<TraceProcessResponse>, ITraceRepository
    {
        public TraceRepository(CosmosInitializer cosmosInitializer, string collectionName) : base(cosmosInitializer, collectionName)
        {

        }

    }
}
