using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PowerBI_Backup
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            
            string powerbi_grant_type = "client_credentials";
            string powerbi_app_uri = @"https://api.powerbi.com/";
            string powerbi_resource = @"https://analysis.windows.net/powerbi/api";
            string powerbi_layout_path = "Report/Layout";

            string powerbi_client_id = data?.powerbi_client_id;
            string powerbi_client_secret = data?.powerbi_client_secret;
            string powerbi_tenant_id = data?.powerbi_tenant_id;



            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            string responseMessage = $"powerbi_client_id:{powerbi_client_id}  powerbi_client_secret:{powerbi_client_secret}  powerbi_tenant_id:{powerbi_tenant_id}";

            return new OkObjectResult(responseMessage);
        }
    }
}
