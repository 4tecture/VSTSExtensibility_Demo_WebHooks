using DemoWorkItemEventHandler.VSTSClient;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Payloads;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoWorkItemEventHandler.WebHooks
{
    public class VstsWebHookHandlercs : VstsWebHookHandlerBase
    {
        public async override Task ExecuteAsync(WebHookHandlerContext context,
                                                    WorkItemUpdatedPayload payload)
        {
            if (payload.Resource.Fields.SystemState.OldValue == "New" && // sample check
                payload.Resource.Fields.SystemState.NewValue == "Approved")
            {
                // your logic goes here...
               
                var projectId = Guid.Parse(payload.ResourceContainers.Project.Id);
                var wiId = payload.Resource.WorkItemId;
                var pat = ConfigurationManager.AppSettings.Get("VSTS_PAT");
                var vstsurl = ConfigurationManager.AppSettings.Get("VSTS_URL");

                using (var service = new WorkItemService(vstsurl, projectId, pat))
                {
                    await service.AddPBITasks(wiId);
                }
            }
        }
    }
}