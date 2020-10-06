using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Saint.Seiya.Shared.Models.Config;
using System;

namespace Saint.Seiya.Infrastructure
{
    public class CosmosInitializer
    {
        private readonly CosmosDBConfig configuration;
        public string Database { get; set; }

        public CosmosInitializer(IOptions<CosmosDBConfig> configuration) : this(configuration.Value)
        {
        }

        public CosmosInitializer(CosmosDBConfig config)
        {
            this.configuration = config;
            this.Database = config.Database;
        }

        public CosmosClient GetClient()
        {
            var client = new CosmosClient(configuration.Endpoint, configuration.AuthKey,
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Direct,
                    MaxRetryAttemptsOnRateLimitedRequests = configuration.MaxRetriesOnThrottling,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(configuration.MaxRetryWaitTimeInSeconds)
                }
            );

            return client;
        }
    }
}
