using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Saint.Ikki.Fx.ApplicationServices.Abstract.Clients;
using Saint.Ikki.Fx.Common;
using Saint.Ikki.Fx.Shared.Models;
using Saint.Ikki.Fx.Shared.Models.Infrastructure;
using Saint.Ikki.Fx.Utils;
using Saint.Ikki.Fx.Utils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.SubOrchestators
{
    public class SuborchestratorUpdateFile
    {

        private const string functionName = "Suborchestrator_UpdateFile";
        private const string activityGetDocumentsIkki = "ActivityDocumentsIkki";
        private const string activityStatus = "UpdateStatus";

        private readonly AppSettings appSettings;
        private readonly ISeiyaApiClient seiyaApiClient;
        private readonly Queries queries;
        private readonly ILogger log;
        private readonly IHttpContextAccessor httpContextAccesor;

        public SuborchestratorUpdateFile(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccesor, Queries queries, ISeiyaApiClient seiyaApiClient)
        {
            this.appSettings = appSettings.Value;
            this.seiyaApiClient = seiyaApiClient;
            this.httpContextAccesor = httpContextAccesor;
            this.queries = queries;
            log = ApplicationLogging.LoggerFactory.CreateLogger<SuborchestratorTest>();
        }


        [FunctionName(functionName)]
        public async Task<bool> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var model = context.GetInput<Shared.Models.RequestModel>();

            log.LogInformation($"{functionName} | Type:INFORMATION | Message: Started orchestration | {model.UniqueId}");

            var statusRequest = new UpdateStatusRequest()
            {
                ProcessType = model.Type,
                Status = ProcessStatus.WithErrors,
                UniqueId = model.UniqueId
            };

            bool result = false;

            try
            {
                var elements = await context.CallActivityAsync<Shared.Models.RequestModel>(activityGetDocumentsIkki, model);

                var error = new List<SPItem>();

                if (elements.Items?.Any() == true)
                {
                  log.LogInformation($"{functionName} | Type:INFORMATION | Message: {elements.Items.Count} processed in Santuario with {error.Count} errors | {model.UniqueId}");
                }
                else
                {
                    log.LogInformation($"{functionName} | Type:INFORMATION | Message: No elements processed | {model.UniqueId}");
                }

                statusRequest.Status = ProcessStatus.Completed;
                result = true;
                log.LogInformation($"{functionName} | Type:INFORMATION | Message: Process {model.Type} completed");
            }
            catch (Exception ex)
            {
                log.LogInformation($"{functionName} | Type:ERROR | Message: {ex.Message} | {model.UniqueId}");
            }
            finally
            {
                await context.CallActivityAsync<bool>(activityStatus, statusRequest);
            }


            return result;
        }

        [FunctionName(activityGetDocumentsIkki)]
        public Shared.Models.RequestModel ActivityGetDocumentsIkki([ActivityTrigger] IDurableActivityContext context)
        {
            var credentials = new System.Net.NetworkCredential(appSettings.SharepointConfig.UserName, appSettings.SharepointConfig.Password);

            var model = context.GetInput<Shared.Models.RequestModel>();

            using var contextSpo = new ClientContext(appSettings.SharepointConfig.Url)
            {
                Credentials = credentials
            };

            var list = contextSpo.Web.Lists.GetByTitle(appSettings.SharepointConfig.DocumentFolder);

            contextSpo.Load(list);
            contextSpo.ExecuteQuery();
            var query = new CamlQuery
            {
                ViewXml = queries.QueryDocumentsIkki()
            };

            contextSpo.ClearElements(model, list, query, functionName, log);

            return model;
        }
    }
}
