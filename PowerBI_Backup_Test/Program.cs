using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.PowerBI.Api.Models;

using System.Threading;
using Enkelt.Connector.PowerBI;
using System.Collections.Generic;
using EnkeltBKP.PowerBI.Model;

namespace PowerBI_Backup_Test
{
    class Program
    {
        //static Enkelt.Connector.PowerBI.Operation powerBIopt = null;
        //static Enkelt.Connector.AzureBlob.Operation blobOpt = null;

        static void Main(string[] args)
        {

            //TokenProvider provider = new TokenProvider("6a71ae02-04e2-4f2f-9cf1-a5a3912121f4", "DRh?Z[@4Crm3v9fdDwyXh=HBpfbb04mR", "7ddcd412-aa67-423e-b6d7-8a90efbeeeb9");
            //powerBIopt = new Enkelt.Connector.PowerBI.Operation(provider);

            //blobOpt = new Enkelt.Connector.AzureBlob.Operation(
            //    "DefaultEndpointsProtocol=https;AccountName=ikeabackup01;AccountKey=4Sf/99+rdVEQwgefpHhzK8JL45Qg729voTXxrnUr4OHwLz8sWNwHeGVZ/XEbSxL3Mb5QW/N2vO9fWqFFPizsZA==;EndpointSuffix=core.windows.net",
            //    "powerbi-backup");

            //var workspaces = powerBIopt.GetGroups().Result;

            //foreach (var workspace in workspaces)
            //{
            //    var reports = powerBIopt.GetReports(workspace.Id);

            //    var tasks = new List<Task>();

            //    foreach (var report in reports.Result)
            //    {
            //        // Start the thread with a ParameterizedThreadStart.
            //        viewModel viewModel = new viewModel
            //        {
            //            report = report,
            //            workspace = workspace
            //        };

            //        tasks.Add(Task.Factory.StartNew(Program.UploadReport, viewModel));
            //    }

            //    // Wait for all the tasks to finish.
            //    Task.WaitAll(tasks.ToArray());
            //}

            //Console.WriteLine("Done Upload!");

            PowerBIConnection powerBIConnection = new PowerBIConnection("6a71ae02-04e2-4f2f-9cf1-a5a3912121f4", "DRh?Z[@4Crm3v9fdDwyXh=HBpfbb04mR", "7ddcd412-aa67-423e-b6d7-8a90efbeeeb9");
            AzureBlobConnection azureBlobConnection = new AzureBlobConnection("DefaultEndpointsProtocol=https;AccountName=ikeabackup01;AccountKey=4Sf/99+rdVEQwgefpHhzK8JL45Qg729voTXxrnUr4OHwLz8sWNwHeGVZ/XEbSxL3Mb5QW/N2vO9fWqFFPizsZA==;EndpointSuffix=core.windows.net", "powerbi-backup");

            BackupConfiguration backupConfiguration = new BackupConfiguration();
            backupConfiguration.Folder = "IKEA PowerBI Backup";
            backupConfiguration.AddTimePostfix = false;

            EnkeltBKP.PowerBI.Service service = new EnkeltBKP.PowerBI.Service(backupConfiguration, powerBIConnection, azureBlobConnection);
            service.RunAll();
            
            Console.WriteLine("Done Upload!");
        }

        //public static void UploadReport(object x)
        //{
        //    var vm = (viewModel)x;
        //    Console.WriteLine($"-------------------{vm.workspace.Name}-----------------");

        //    var file = powerBIopt.ExportReport(vm.workspace.Id, vm.report.Id).Result;

        //    if (file != null)
        //    {
        //        string localPath = $"./Backup/{vm.workspace.Name}/{vm.report.Name}/";
        //        string fileName = $"{vm.report.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.pbix";

        //        var blobResult = blobOpt.UploadBlobAsync(localPath, fileName, file).Result;
        //    }

        //    Console.WriteLine($"Done Upload {vm.workspace.Name} -> {vm.report.Name}!");
        //}
    }

    //public class viewModel
    //{
    //    public Group workspace { get; set; }
    //    public Report report { get; set; }
    //}
}
