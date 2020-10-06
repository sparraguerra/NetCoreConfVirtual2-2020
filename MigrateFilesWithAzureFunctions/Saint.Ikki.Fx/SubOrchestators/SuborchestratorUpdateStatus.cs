using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Saint.Ikki.Fx.Common;
using Saint.Ikki.Fx.Shared.Models;
using System.Threading.Tasks;

namespace Saint.Ikki.Fx.SubOrchestators
{
    public class SuborchestratorUpdateStatus
    {
        private const string activityStatus = "UpdateStatus";
        private readonly StatusUpdater statusUpdater;


        public SuborchestratorUpdateStatus(StatusUpdater statusUpdater)
        {
            this.statusUpdater = statusUpdater;
        }

        [FunctionName(activityStatus)]
        public async Task<bool> ActivityStatus([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var model = context.GetInput<UpdateStatusRequest>();

            return await statusUpdater.UpdateStatus(model, log);
        }
    }
}
