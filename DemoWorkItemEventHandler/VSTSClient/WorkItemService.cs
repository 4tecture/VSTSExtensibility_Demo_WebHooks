using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DemoWorkItemEventHandler.VSTSClient
{
    public class WorkItemService : IDisposable
    {
        private VssConnection connection;
        private Guid projectId;
        private WorkItemTrackingHttpClient witClient;

        public WorkItemService(string collectionUri, Guid projectId, string pat)
        {
            connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, pat));
            witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            this.projectId = projectId;
        }

        public async Task AddPBITasks(int pbiId)
        {
            try
            {
                var pbi = await witClient.GetWorkItemAsync(pbiId);
                await CreateChildTask(pbi, "Do something...");
                await CreateChildTask(pbi, "Do something more...");
            }
            catch (Exception ex)
            {
                // who cares! ;-)
            }
        }

        private async Task<WorkItem> CreateChildTask(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem parentPbi, string title)
        {
            var document = new JsonPatchDocument();
            document.Add(
                new JsonPatchOperation()
                {
                    Path = "/fields/System.Title",
                    Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                    Value = title
                });

            document.Add(
                new JsonPatchOperation()
                {
                    Path = "/relations/-",
                    Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                    Value = new
                    {
                        rel = "System.LinkTypes.Hierarchy-Reverse",
                        url = parentPbi.Url
                    }
                    //https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workItems/297
                });

            return await witClient.CreateWorkItemAsync(document, projectId, "Task");
        }

        public void Dispose()
        {
            witClient.Dispose();
            connection.Disconnect();
            witClient = null;
            connection = null;
        }
    }
}


