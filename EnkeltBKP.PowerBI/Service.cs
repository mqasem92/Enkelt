using Enkelt.Connector.PowerBI;
using EnkeltBKP.PowerBI.Model;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnkeltBKP.PowerBI
{
    public class Service
    {
        public BackupConfiguration BackupConfiguration { get; set; }


        private PowerBIConnection PowerBIConnection { get; set; }
        private AzureBlobConnection AzureBlobConnection { get; set; }


        private Enkelt.Connector.PowerBI.Operation PowerBIOperation { get; set; }
        private Enkelt.Connector.AzureBlob.Operation AzureBlobOperation { get; set; }


        public Service(PowerBIConnection powerBIConnection, AzureBlobConnection azureBlobConnection)
        {
            BackupConfiguration = new BackupConfiguration();
            PowerBIConnection = powerBIConnection;
            AzureBlobConnection = azureBlobConnection;

            SetConnections();
        }

        public Service(BackupConfiguration backupConfiguration, PowerBIConnection powerBIConnection, AzureBlobConnection azureBlobConnection)
        {
            BackupConfiguration = backupConfiguration;
            PowerBIConnection = powerBIConnection;
            AzureBlobConnection = azureBlobConnection;

            SetConnections();
        }

        /// <summary>
        /// Start backup all workspace and reports
        /// </summary>
        /// <returns></returns>
        public bool RunAll()
        {
            var workspaces = PowerBIOperation.GetGroups().Result;

            var workspaceTasks = new List<Task>();

            foreach (var workspace in workspaces)
            {
                workspaceTasks.Add(Task.Factory.StartNew(this.UploadReports, workspace));
            }

            // Wait for all the tasks to finish.
            Task.WaitAll(workspaceTasks.ToArray());

            return true;
        }

        /// <summary>
        /// Set PowerBI and Blob Stoeage connection settings
        /// </summary>
        private void SetConnections()
        {
            TokenProvider provider = new TokenProvider(PowerBIConnection.GrantType, PowerBIConnection.ClientId, PowerBIConnection.Resource, PowerBIConnection.ClientSecret, PowerBIConnection.TenantId);
            PowerBIOperation = new Enkelt.Connector.PowerBI.Operation(provider);

            AzureBlobOperation = new Enkelt.Connector.AzureBlob.Operation(AzureBlobConnection.ConnectionString, AzureBlobConnection.ContainerName);
        }

        private void UploadReportFile(object uploadSet)
        {
            var vm = (UploadSetViewModel)uploadSet;
            var file = PowerBIOperation.ExportReport(vm.Workspace.Id, vm.Report.Id).Result;

            if (file != null)
            {
                var datePostfix = BackupConfiguration.AddDatePostfix ? DateTime.Now.ToString("_yyyyMMdd") : "";
                var timePostfix = BackupConfiguration.AddTimePostfix ? DateTime.Now.ToString("_HHmmss") : "";

                string localPath = $"./{BackupConfiguration.Folder}/{vm.Workspace.Name}/{vm.Report.Name}/";
                string fileName = $"{vm.Report.Name}{datePostfix}{timePostfix}.{BackupConfiguration.FileExtension}";

                var blobResult = AzureBlobOperation.UploadBlobAsync(localPath, fileName, file).Result;
            }
        }

        private void UploadReports(object group)
        {
            var workspace = (Group)group;
            var reports = PowerBIOperation.GetReports(workspace.Id);
            var reportTasks = new List<Task>();

            foreach (var report in reports.Result)
            {
                // Start the thread with a ParameterizedThreadStart.
                UploadSetViewModel uploadSetViewModel = new UploadSetViewModel
                {
                    Report = report,
                    Workspace = workspace
                };

                reportTasks.Add(Task.Factory.StartNew(this.UploadReportFile, uploadSetViewModel));
            }

            // Wait for all the tasks to finish.
            Task.WaitAll(reportTasks.ToArray());
        }
    }
}
