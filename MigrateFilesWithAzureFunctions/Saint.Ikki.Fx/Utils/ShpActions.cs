using Microsoft.Extensions.Logging;
using Microsoft.SharePoint.Client;
using Saint.Ikki.Fx.Shared.Models;
using System;

namespace Saint.Ikki.Fx.Utils
{
    public static class ShpActions
    {
        private static readonly bool toRecycleBin = Convert.ToBoolean(Environment.GetEnvironmentVariable("ToRecycleBin"));

        public static void GetElements(this ClientContext contextSpo, RequestModel model, List list, CamlQuery query, string functionName, ILogger log)
        {
            var docs = list.GetItems(query);

            contextSpo.Load(docs);
            contextSpo.ExecuteQuery();

            if (docs.Count > 0)
            {
                foreach (var doc in docs)
                {
                    if (doc.FileSystemObjectType == FileSystemObjectType.File)
                    {
                        var f = doc.File;
                        var listItem = f.ListItemAllFields;
                        contextSpo.Load(f);
                        contextSpo.Load(listItem);

                        listItem["SeiyaStatus"] = StatusProcessor.Procesando;
                        listItem.Update();
                        ExecuteQuery(contextSpo);
                        contextSpo.ExecuteQuery();
                        var sID = listItem["ID"].ToString();
                        var item = new SPItem
                        {
                            Id = sID
                        };

                        model.Items.Add(item);

                        log.LogInformation($"{functionName} | Type:INFORMATION | Message: Added {sID} for processing | {model.UniqueId}");
                    }
                }
            }
            else
            {
                log.LogInformation($"{functionName} | Type:INFORMATION | Message: Documents not found | {model.UniqueId}");
            }
        }

        public static void ClearElements(this ClientContext contextSpo, RequestModel model, List list, CamlQuery query, string functionName, ILogger log)
        {
            var docs = list.GetItems(query);

            contextSpo.Load(docs);
            contextSpo.ExecuteQuery();

            if (docs.Count > 0)
            {
                foreach (var doc in docs)
                {
                    if (doc.FileSystemObjectType == FileSystemObjectType.File)
                    {
                        var f = doc.File;
                        var listItem = f.ListItemAllFields;
                        contextSpo.Load(f);
                        contextSpo.Load(listItem);

                        listItem["SeiyaStatus"] = string.Empty; 
                        listItem["SeiyaId"] = string.Empty;
                        listItem["SeiyaPartitionKey"] = string.Empty;
                        listItem.Update();
                        ExecuteQuery(contextSpo);
                        contextSpo.ExecuteQuery();
                        var sID = listItem["ID"].ToString();
                        var item = new SPItem
                        {
                            Id = sID
                        };

                        model.Items.Add(item);

                        log.LogInformation($"{functionName} | Type:INFORMATION | Message: {sID} reset | {model.UniqueId}");
                    }
                }
            }
            else
            {
                log.LogInformation($"{functionName} | Type:INFORMATION | Message: Documents not found | {model.UniqueId}");
            }
        }

        public static void DeleteItem(ListItem listItem, string functionName, ILogger log)
        {
            try
            {
                if (toRecycleBin)
                {
                    listItem.Recycle();
                }
                else
                {
                    listItem.DeleteObject();
                }
            }
            catch (Exception e)
            {
                log.LogInformation($"{functionName} | Type:ERROR | Message: {e.Message}");

                throw;
            }
        }

        public static void ExecuteQuery(this ClientContext contextSpo)
        {
            if (contextSpo.HasPendingRequest)
            {
                contextSpo.ExecuteQueryAsync()
                .Wait();
            }
        }
    }
}
