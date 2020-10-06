using Autofac;
using Saint.Seiya.Domain.Abstract;
using Saint.Seiya.Domain.Middleware;
using System.Net.Http;

namespace Saint.Seiya.Domain.Infrastructure.AutofacModules
{
    public class DomainModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {          
            builder.RegisterType<DocumentDomain>().As<IDocumentDomain>(); 
            builder.RegisterType<MultipartRequestMiddleware>();


            builder.RegisterType<HttpClient>();
        }
    }
}
