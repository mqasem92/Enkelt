using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Enkelt.Connector.PowerBI
{
    public class TokenProvider
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string Resource { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }

        public TokenProvider(string grantType, string clientId, string resource, string clientSecret, string tenantId)
        {
            this.GrantType = grantType;
            this.ClientId = clientId;
            this.Resource = resource;
            this.ClientSecret = clientSecret;
            this.TenantId = tenantId;
        }

        public async Task<string> GetAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var requestBody = $"resource={HttpUtility.UrlEncode(Resource)}" +
                                  $"&client_id={ClientId}" +
                                  $"&grant_type={GrantType}" +
                                  $"&client_secret={ClientSecret}";

                httpClient.Timeout = new TimeSpan(0, 0, 60);

                using (var response = await httpClient.PostAsync($"https://login.microsoftonline.com/{TenantId}/oauth2/token",
                    new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded")).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                        string _accessToken = result.Value<string>("access_token");
                        return _accessToken;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }
    }
}
