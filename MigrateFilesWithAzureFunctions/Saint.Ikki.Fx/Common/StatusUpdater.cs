using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saint.Ikki.Fx.ApplicationServices.Abstract.Clients;
using Saint.Ikki.Fx.Shared.Models;
using Saint.Ikki.Fx.Shared.Models.Config;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.Common
{
    public class StatusUpdater
    {
        private readonly ISeiyaApiClient seiyaApiClient;

        public StatusUpdater(ISeiyaApiClient seiyaApiClient)
        {
            this.seiyaApiClient = seiyaApiClient;
        }

        public async Task<bool> UpdateStatus(UpdateStatusRequest model, ILogger log)
        {
            try
            {
                return await seiyaApiClient.UpdateProcessAsync(model);
            }
            catch (Exception ex)
            {
                log.LogInformation($"StatusUpdater | Type:ERROR | Message: {ex.Message}");
                throw;
            }
        }
    }
}
