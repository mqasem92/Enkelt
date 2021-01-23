using System;
using System.Collections.Generic;
using System.Text;

namespace EnkeltBKP.PowerBI.Model
{
    public class PowerBIConnection
    {
        public string GrantType { get; set; } = "client_credentials";
        public string ClientId { get; set; }
        public string Resource { get; set; } = "https://analysis.windows.net/powerbi/api";
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }

        public PowerBIConnection(string grantType, string clientId, string resource, string clientSecret, string tenantId)
        {
            GrantType = grantType;
            ClientId = clientId;
            Resource = resource;
            ClientSecret = clientSecret;
            TenantId = tenantId;
        }

        public PowerBIConnection(string clientId, string clientSecret, string tenantId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            TenantId = tenantId;
        }
    }
}
