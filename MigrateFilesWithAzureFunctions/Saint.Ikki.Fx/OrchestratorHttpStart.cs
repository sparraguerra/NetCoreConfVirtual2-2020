using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Saint.Ikki.Fx.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static Saint.Ikki.Fx.Shared.Models.Common.Constants;

namespace Saint.Ikki.Fx
{
    public class OrchestratorHttpStart
    {
        private const string functionName = "Orchestrator_HttpStart";
        private const string functionNameOrchestrator = "Orchestrator";

        [FunctionName(functionName)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            var model = await req.Content.ReadAsAsync<RequestModel>();

            log.LogInformation($"{functionName} | Type:INFORMATION | Message: Started by {model.User} with {GetActivity(model.Type)} option selected | {model.UniqueId}");

            await starter.StartNewAsync(functionNameOrchestrator, model);

            return new OkObjectResult($"{functionName} | Type:INFORMATION | Message: Started by {model.User} with {GetActivity(model.Type)} option selected | {model.UniqueId}");
        }

        [FunctionName(functionNameOrchestrator)]
        public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var model = context.GetInput<RequestModel>();

            log.LogInformation($"{functionNameOrchestrator} | Type:INFORMATION | Message: Started orchestration | {model.UniqueId}");

            bool response = false;

            try
            {
                response = await context.CallSubOrchestratorAsync<bool>(GetActivity(model.Type), model);
            }
            catch (Exception ex)
            {
                log.LogInformation($"{GetActivity(model.Type)} | Type:ERROR | Message: {ex.Message} | {model.UniqueId}");
            }

            log.LogInformation($"{functionNameOrchestrator} | Type:INFORMATION | Message: Finish orchestration {GetActivity(model.Type)} with Status {response} | {model.UniqueId}");
        }

        private static string GetActivity(int key)
        {
            var collection = new Dictionary<int, string>()
                                { 
                                    {1, OrchestatorHttpCalls.Migrate},
                                    {2, OrchestatorHttpCalls.ClearDocs }
                                };

            if (collection.TryGetValue(key, out string result))
            {
                return result;
            }

            return "NOTFOUND";
        }
    }
}
