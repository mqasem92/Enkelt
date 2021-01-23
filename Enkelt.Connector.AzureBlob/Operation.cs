using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Enkelt.Connector.AzureBlob
{
    public class Operation
    {
        private BlobServiceClient BlobServiceClient { get; set; }
        private BlobContainerClient ContainerClient { get; set; }

        public Operation(string connectionString, string containerName)
        {
            BlobServiceClient = new BlobServiceClient(connectionString);

            ContainerClient = BlobServiceClient.GetBlobContainerClient(containerName);
            ContainerClient.CreateIfNotExists();
        }

        public async Task<Response<BlobContentInfo>> UploadBlobAsync(string localPath, string fileName, Stream file)
        {
            try
            {
                // Create a local file in the ./data/ directory for uploading and downloading
                string localFilePath = Path.Combine(localPath, fileName);

                // Get a reference to a blob
                BlobClient blobClient = ContainerClient.GetBlobClient(localFilePath);

                //upload its data
                return await blobClient.UploadAsync(file, true);
            }
            catch(Exception ex)
            {
                throw new Exception($"File Upload Field [{fileName}] to Blob Storage  InnerException: {ex.InnerException}");
            }
        }
    }
}
