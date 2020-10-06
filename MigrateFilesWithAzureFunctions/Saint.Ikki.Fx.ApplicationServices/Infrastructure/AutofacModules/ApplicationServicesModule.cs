using Autofac;
using Saint.Ikki.Fx.ApplicationServices.Clients;
using System.Net.Http;

namespace Saint.Ikki.Fx.ApplicationServices.Infrastructure.AutofacModules
{
    public class ApplicationServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        { 

            builder.RegisterType<SeiyaApiClient>().WithParameter(
                                                    (p, ctx) => p.ParameterType == typeof(HttpClient),
                                                    (p, ctx) => ctx.Resolve<IHttpClientFactory>().CreateClient("seiya"));

        }
    }
}
