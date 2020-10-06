using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saint.Ikki.Fx.ApplicationServices.Abstract.Clients;
using Saint.Ikki.Fx.Common;
using Saint.Ikki.Fx.Shared.Models;
using Saint.Ikki.Fx.Shared.Models.Infrastructure;
using Saint.Ikki.Fx.Shared.Models.Seiya;
using Saint.Ikki.Fx.Utils;
using Saint.Ikki.Fx.Utils.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.SubOrchestators
{
    public class SuborchestratorTest
    {
       
        private const string functionName = "Suborchestrator_Test";
        private const string activitySanctuary = "ActivitySanctuary";
        private const string activityGetDocuments = "ActivityGetDocuments";
        private const string activityStatus = "UpdateStatus";

        private readonly AppSettings appSettings; 
        private readonly ISeiyaApiClient seiyaApiClient;
        private readonly Queries queries;
        private readonly ILogger log;
        private readonly IHttpContextAccessor httpContextAccesor;

        public SuborchestratorTest(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccesor, Queries queries, ISeiyaApiClient seiyaApiClient)
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
                var elements = await context.CallActivityAsync<Shared.Models.RequestModel>(activityGetDocuments, model);

                var error = new List<SPItem>();
                var inSantuario = new List<SPItem>();

                if (elements.Items?.Any() == true)
                {
                    foreach (var item in elements.Items)
                    {
                        try
                        {
                            await CallSaintSeiya(context, model, error, inSantuario, item);
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation($"{functionName} | Type:ERROR | Messsage: {ex.Message} | {model.UniqueId}");
                        }
                    }

                    log.LogInformation($"{functionName} | Type:INFORMATION | Message: {inSantuario.Count} processed in Santuario with {error.Count} errors | {model.UniqueId}");
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

        [FunctionName(activityGetDocuments)]
        public Shared.Models.RequestModel ActivityGetDocuments([ActivityTrigger] IDurableActivityContext context)
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
                ViewXml = queries.QueryDocuments(appSettings.SharepointConfig.RowLimit.ToString())
            };

            contextSpo.GetElements(model, list, query, functionName, log);

            return model;
        }

        private async Task CallSaintSeiya(IDurableOrchestrationContext context, Shared.Models.RequestModel model, List<SPItem> error, 
                                            List<SPItem> inSantuario, SPItem item)
        {
            item.UniqueId = model.UniqueId;
            item.User = model.User;

            var response = await context.CallActivityAsync<Shared.Models.RequestModel>(activitySanctuary, item);

            if (response.Items?.Any() == true)
            {
                var element = response.Items.FirstOrDefault();
                if (element.Processed)
                {
                    
                    inSantuario.Add(element);
                    log.LogInformation($"{functionName} | Type:INFORMATION | Status:OK | Message: ID {element.Id} " +
                        $"processed document with SeiyaID {response.Items.FirstOrDefault().Id} NAME {response.Items.FirstOrDefault().Name} | UNIQUEID: {model.UniqueId}");
                }
                else
                {
                    error.Add(element);

                    log.LogInformation($"{functionName} | Type:ERROR | Status:KO | Message: ID {element.Id} failed send to Sanctuary with NAME " +
                        $"{response.Items.FirstOrDefault().Name} | UNIQUEID: {model.UniqueId}");
                }
            }
        }

        [FunctionName(activitySanctuary)]
        public async Task<Shared.Models.RequestModel> ActivitySanctuary([ActivityTrigger] SPItem item)
        {
            var credentials = new System.Net.NetworkCredential(appSettings.SharepointConfig.UserName, appSettings.SharepointConfig.Password);

            var result = new Shared.Models.RequestModel();

            using var contextSpo = new ClientContext(appSettings.SharepointConfig.Url)
            {
                Credentials = credentials
            };
            var list = contextSpo.Web.Lists.GetByTitle(appSettings.SharepointConfig.DocumentFolder);
            var query = new CamlQuery()
            {
                ViewXml = queries.GetDocsById(item.Id)
            };

            var docs = list.GetItems(query);

            contextSpo.Load(docs);
            contextSpo.ExecuteQuery();

            if (docs.Count > 0)
            {
                var i = 1;

                foreach (var doc in docs)
                {
                    if (doc.FileSystemObjectType == FileSystemObjectType.File)
                    {
                        var f = doc.File;
                        var listItem = f.ListItemAllFields;

                        contextSpo.Load(f);
                        contextSpo.Load(listItem);
                        contextSpo.ExecuteQuery();

                        item.Name = listItem["FileLeafRef"].ToString();

                        var failed = false;

                        try
                        {
                            var metadata = Metadata.BindingTypes(listItem);
                            metadata.SessionId = item.UniqueId;
                            var stream = f.OpenBinaryStream();
                            contextSpo.ExecuteQuery();

                            log.LogInformation($"{functionName} | Type:INFORMATION | Message: Sending file {f.Name} to Santuario {i++} of {docs.Count()} | {item.UniqueId}");
 
                            var requestSeiya = MakeRequest(metadata, item.User);
                            using var memoryStream = new MemoryStream();
                            await stream.Value.CopyToAsync(memoryStream); 
                            memoryStream.Position = 0;

                            var response = await seiyaApiClient.IndexBinaryDocumentAsync(requestSeiya, memoryStream, metadata.Name);
                                                     
                            if (response != null)
                            {
                                item.Processed = true;

                                listItem = UpdateStatus(listItem, response, StatusProcessor.Procesado);
                                listItem.Update();

                                log.LogInformation($"{functionName} | Type:INFORMATION | Message: Document {item.Id} Processed in Santuario with Id: {response.Id} - " +
                                   $"PartitionKey: {response.PartitionKey} {item.Name} | {item.UniqueId}");
                            }
                            else
                            {
                                log.LogInformation($"{functionName} | Type:ERROR | Message: Document {item.Id} FAILED. Added to errors - Response {false} " +
                                    $"from Sanctuary | {item.UniqueId}");
                                item.Processed = false;
                                response = new ResponseModel()
                                {
                                    Id = string.Empty,
                                    PartitionKey = string.Empty
                                };

                                listItem = UpdateStatus(listItem, response, StatusProcessor.NoProceso);
                                listItem.Update();

                                failed = true;
                            }

                            contextSpo.ExecuteQuery();
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation($"{functionName} | Type:ERROR | Message: Failed document {ex.Message} | {item.UniqueId}");

                            var response = new ResponseModel()
                            {
                                Id = string.Empty,
                                PartitionKey = string.Empty,
                            };

                            listItem = UpdateStatus(listItem, response, StatusProcessor.NoProceso);
                            listItem.Update();

                            contextSpo.ExecuteQuery();

                            failed = true;
                        } 

                        result.Items.Add(item);

                        if (failed)
                        {
                            result.Items.FirstOrDefault().Processed = false;
                        }
                    }
                }
            }

            return result;
        }

        private ListItem UpdateStatus(ListItem listItem, ResponseModel response, string procesado)
        {
            listItem["SeiyaStatus"] = procesado;
            listItem["SeiyaId"] = response.Id;
            listItem["SeiyaPartitionKey"] = response.PartitionKey;
            return listItem;
        }

        private IndexBinaryRequestModel MakeRequest(object metadata, string user)
        {
            return new IndexBinaryRequestModel
            {
                DocumentClass = appSettings.SeiyaApiClientConfig.DocumentClass,
                DocumentClassId = appSettings.SeiyaApiClientConfig.DocumentClassId,
                DocumentType = appSettings.SeiyaApiClientConfig.DocumentType,
                User = user,
                Properties = JObject.Parse(JsonConvert.SerializeObject(metadata))
            };
        } 
    }
}
