using Autofac;
using Microsoft.Extensions.Options;
using Saint.Seiya.Infrastructure.Abstract;
using Saint.Seiya.Shared.Models.Config;

namespace Saint.Seiya.Infrastructure.Infrastructure.AutofacModules
{ 
    public class InfrastructureModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.Register<DocumentRepository>((ctx, p) =>
            {
                var cosmosConfig = ctx.Resolve<IOptions<CosmosDBConfig>>().Value;
                
                var cosmosInitializer = new CosmosInitializer(cosmosConfig);
                return new DocumentRepository(cosmosInitializer, cosmosConfig.Collection);
            }).As<IDocumentRepository>().SingleInstance();

            builder.Register<TraceRepository>((ctx, p) =>
            {
                var cosmosConfig = ctx.Resolve<IOptions<CosmosDBConfig>>().Value;

                var cosmosInitializer = new CosmosInitializer(cosmosConfig);
                return new TraceRepository(cosmosInitializer, cosmosConfig.TracesCollection);
            }).As<ITraceRepository>().SingleInstance();


            builder.Register<ProcessRepository>((ctx, p) =>
            {
                var cosmosConfig = ctx.Resolve<IOptions<CosmosDBConfig>>().Value;

                var cosmosInitializer = new CosmosInitializer(cosmosConfig);
                return new ProcessRepository(cosmosInitializer, cosmosConfig.ProcessesCollection);
            }).As<IProcessRepository>().SingleInstance();


            builder.RegisterType<BlobStorageRepository>().As<IBlobStorageRepository>();
        }
    }
}
