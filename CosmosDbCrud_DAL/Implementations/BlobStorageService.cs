using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CosmosDbCrud_DAL.Services
{/// <summary>
 /// Service class to interact with Azure Blob Storage for file upload.
 /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly CloudBlobContainer _blobContainer;

        /// <summary>
        /// Initializes a new instance of the BlobStorageService class.
        /// </summary>
        public BlobStorageService(IConfiguration config)
        {

            // Read the connection string and container name from environment variables
            string connectionString = config["Values:blobConnectionString"];
            string containerName = config["Values:blobContainerName"];
           
            // Create a CloudBlobContainer reference based on the provided connection string and container name
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference(containerName);
        }


        /// <summary>
        /// Uploads a file to Azure Blob Storage asynchronously.
        /// </summary>
        /// <param name="file">The file to be uploaded as an IFormFile.</param>
        /// <returns>The URI of the uploaded blob as a string.</returns>
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // Generate a unique file name with a random GUID and its original file extension

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(fileName);

            // Upload the file's stream to the blob storage asynchronously

            using (Stream stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);
            }

            // Return the URI of the uploaded blob as a string
            return blob.Uri.ToString();
        }
    }
}
