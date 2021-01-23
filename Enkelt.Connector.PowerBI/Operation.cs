using Ionic.Zip;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Enkelt.Connector.PowerBI
{
    public class Operation
    {
        #region Private 

        private TokenProvider TokenProvider { get; set; }

        private string Token { get { return TokenProvider.GetAccessToken().Result; } }

        #endregion

        #region private Property

        private string AppUri { get { return "https://api.powerbi.com/"; } }

        #endregion

        public Operation(TokenProvider provider)
        {
            TokenProvider = provider;
        }

        #region Group

        public async Task<List<Group>> GetGroups()
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var groupId = (await client.Groups.GetGroupsAsync().ConfigureAwait(false)).Value;

                return groupId.ToList();
            }
        }

        public async Task<Group> GetGroupById(string groupId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = (await client.Groups.GetGroupsAsync($"id eq '{groupId}'", 1).ConfigureAwait(false)).Value.FirstOrDefault();

                return item;
            }
        }

        public async Task<List<GroupUser>> GetGroupUsers(Guid groupId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = (await client.Groups.GetGroupUsersAsync(groupId).ConfigureAwait(false)).Value;

                return item.ToList();
            }
        }

        #endregion

        #region Reports

        public async Task<List<Report>> GetReports(Guid groupId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Reports.GetReportsInGroupAsync(groupId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        public async Task<Report> GetReportById(Guid groupId, Guid reportId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = (await client.Reports.GetReportInGroupAsync(groupId, reportId).ConfigureAwait(false));
                return item;
            }
        }

        public async Task<Stream> ExportReport(Guid groupId, Guid reportId)
        {
            try
            {
                using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
                {
                    var item = await client.Reports.ExportReportAsync(groupId, reportId).ConfigureAwait(false);
                    return item;
                }
            }
            catch
            {
                return null;
            }
            
        }

        //public Stream ExportReport(Guid groupId, Guid reportId)
        //{
        //    Stream file = null;
        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

        //        var req = httpClient.GetAsync($"https://api.powerbi.com/v1.0/myorg/groups/{groupId.ToString()}/reports/{reportId.ToString()}/Export").ContinueWith(res =>
        //        {
        //            var result = res.Result;
        //            if (result.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                var readData = result.Content.ReadAsStreamAsync();
        //                readData.Wait();

        //                file = readData.Result;
        //            }
        //        });

        //        req.Wait();


        //        return file;
        //    }
        //}

        public Stream GetReportLayout(Guid groupId, Guid reportId)
        {
            var file = this.ExportReport(groupId, reportId);

            using (ZipFile zip = ZipFile.Read(file.Result))
            {
                foreach (ZipEntry e in zip)
                {
                    if (e.FileName == "Report/Layout")
                    {
                        MemoryStream data = new MemoryStream();
                        e.Extract(data);
                        return data;
                    }
                }
            }

            return null;
        }

        public string GetReportLayoutAsString(Guid groupId, Guid reportId)
        {
            var file = this.ExportReport(groupId, reportId);

            if (file == null)
                return null;

            using (ZipFile zip = ZipFile.Read(file.Result))
            {
                foreach (ZipEntry e in zip)
                {
                    if (e.FileName == "Report/Layout")
                    {
                        MemoryStream data = new MemoryStream();
                        e.Extract(data);

                        var encoding = Encoding.Unicode;
                        return encoding.GetString(data.ToArray());
                    }
                }
            }

            return null;
        }

        #endregion

        #region Reports

        public async Task<List<Dashboard>> GetDashboards(Guid groupId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Dashboards.GetDashboardsInGroupAsync(groupId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        public async Task<Dashboard> GetDashboardById(Guid groupId, Guid dashboardId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = (await client.Dashboards.GetDashboardInGroupAsync(groupId, dashboardId).ConfigureAwait(false));

                return item;
            }
        }

        #endregion

        #region Pages

        public async Task<List<Page>> GetPages(Guid groupId, Guid reportId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Reports.GetPagesInGroupAsync(groupId, reportId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        public async Task<Page> GetPageById(Guid groupId, Guid reportId, string pageName)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = await client.Reports.GetPageInGroupAsync(groupId, reportId, pageName).ConfigureAwait(false);

                return item;
            }
        }

        #endregion

        #region Datasets

        public async Task<List<Dataset>> GetDatasets(Guid groupId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Datasets.GetDatasetsInGroupAsync(groupId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        public async Task<Dataset> GetDatasetById(Guid groupId, string datasetId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var item = (await client.Datasets.GetDatasetInGroupAsync(groupId, datasetId).ConfigureAwait(false));

                return item;
            }
        }

        public async Task<List<Datasource>> GetDatasources(Guid groupId, string datasetId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Datasets.GetDatasourcesInGroupAsync(groupId, datasetId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        public async Task<List<Datasource>> GetDatasourceById(Guid groupId, string datasetId)
        {
            using (var client = new PowerBIClient(new Uri(AppUri), new TokenCredentials(Token, "Bearer")))
            {
                var list = (await client.Datasets.GetDatasourcesAsync(groupId, datasetId).ConfigureAwait(false)).Value;

                return list.ToList();
            }
        }

        #endregion
    }
}
