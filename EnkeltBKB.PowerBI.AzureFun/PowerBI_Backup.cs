using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EnkeltBKP.PowerBI.Model;

namespace EnkeltBKB.PowerBI.AzureFun
{
    public class PowerBI_Backup
    {

        public string PowerBI_ClientID { get { return Environment.GetEnvironmentVariable("PowerBI_ClientID"); } }
        public string PowerBI_ClientSecret { get { return Environment.GetEnvironmentVariable("PowerBI_ClientSecret"); } }
        public string PowerBI_TenantId { get { return Environment.GetEnvironmentVariable("PowerBI_TenantId"); } }
        public string BackupBlob_ConnectionString { get { return Environment.GetEnvironmentVariable("BackupBlob_ConnectionString"); } }
        public string BackupBlob_ContainerName { get { return Environment.GetEnvironmentVariable("BackupBlob_ContainerName"); } }
        public string BackupBlob_FolderName { get { return Environment.GetEnvironmentVariable("BackupBlob_FolderName"); } }


        [FunctionName("RunAll")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //string name = req.Query["name"];
            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);

            try
            {
                log.LogInformation("C# HTTP trigger function [RunAll] processed a request.");

                #region Validation

                log.LogInformation("Start validation application settings.");

                if (string.IsNullOrEmpty(PowerBI_ClientID))
                    return new BadRequestObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", "[PowerBI_ClientID] Not found in application setting!"));

                if (string.IsNullOrEmpty(PowerBI_ClientSecret))
                    return new BadRequestObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", "[PowerBI_ClientSecret] Not found in application setting!"));

                if (string.IsNullOrEmpty(PowerBI_TenantId))
                    return new BadRequestObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", "[PowerBI_TenantId] Not found in application setting!"));

                if (string.IsNullOrEmpty(BackupBlob_ConnectionString))
                    return new BadRequestObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", "[BackupBlob_ConnectionString] Not found in application setting!"));

                if (string.IsNullOrEmpty(BackupBlob_ContainerName))
                    return new BadRequestObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", "[BackupBlob_ContainerName] Not found in application setting!"));

                #endregion

                log.LogInformation("Init PowerBI connection settings.");
                PowerBIConnection powerBIConnection = new PowerBIConnection(PowerBI_ClientID, PowerBI_ClientSecret, PowerBI_TenantId);

                log.LogInformation("Init Azure Blob Storage connection settings.");
                AzureBlobConnection azureBlobConnection = new AzureBlobConnection(BackupBlob_ConnectionString, BackupBlob_ContainerName);

                log.LogInformation("Set required backup configuration.");
                BackupConfiguration backupConfiguration = new BackupConfiguration();

                if (!string.IsNullOrEmpty(BackupBlob_FolderName))
                    backupConfiguration.Folder = BackupBlob_FolderName;

                log.LogInformation("Backup job get started.");
                EnkeltBKP.PowerBI.Service service = new EnkeltBKP.PowerBI.Service(backupConfiguration, powerBIConnection, azureBlobConnection);
                service.RunAll();
                log.LogInformation("Backup job finished.");

                return (ActionResult)new OkObjectResult("{\"IsSuccess\":\"1\",\"Message\":\"Job run successfully\"}");
            }
            catch (Exception ex)
            {
                return (ActionResult)new OkObjectResult(string.Format("{\"IsSuccess\":\"0\",\"Message\":\"{0}\"}", ex.Message));
            }
        }
    }
}
