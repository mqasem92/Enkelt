using System;
using System.Collections.Generic;
using System.Text;

namespace EnkeltBKP.PowerBI.Model
{
    public class AzureBlobConnection
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }

        public AzureBlobConnection(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
        }
    }
}
