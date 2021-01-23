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
    public static class PowerBI_Backup
    {
        [FunctionName("RunAll")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;



            PowerBIConnection powerBIConnection = new PowerBIConnection("6a71ae02-04e2-4f2f-9cf1-a5a3912121f4", "DRh?Z[@4Crm3v9fdDwyXh=HBpfbb04mR", "7ddcd412-aa67-423e-b6d7-8a90efbeeeb9");
            AzureBlobConnection azureBlobConnection = new AzureBlobConnection("DefaultEndpointsProtocol=https;AccountName=ikeabackup01;AccountKey=4Sf/99+rdVEQwgefpHhzK8JL45Qg729voTXxrnUr4OHwLz8sWNwHeGVZ/XEbSxL3Mb5QW/N2vO9fWqFFPizsZA==;EndpointSuffix=core.windows.net", "powerbi-backup");

            BackupConfiguration backupConfiguration = new BackupConfiguration();
            backupConfiguration.Folder = "IKEA PowerBI Backup";
            backupConfiguration.AddTimePostfix = false;

            EnkeltBKP.PowerBI.Service service = new EnkeltBKP.PowerBI.Service(backupConfiguration, powerBIConnection, azureBlobConnection);
            service.RunAll();

            Console.WriteLine("Done Upload!");


            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
