using Autofac;
using Saint.Seiya.Services.Abstract;

namespace Saint.Seiya.Services.Infrastructure.AutofacModules
{    
    public class ServiceModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {     
            builder.RegisterType<DocumentService>().As<IDocumentService>(); 
        }
    }
}
