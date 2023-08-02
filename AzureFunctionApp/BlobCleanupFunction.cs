using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class BlobCleanupFunction
{
    private static TelemetryClient telemetryClient;
    /// <summary>
    /// The Azure Function triggered by a timer to clean up old blobs in a storage container.
    /// </summary>
    /// <param name="myTimer">The timer information provided by the Azure Functions runtime.</param>
    /// <param name="log">Logger instance to log information during the function execution.</param>
    /// <param name="context">The execution context containing information about the function execution environment.</param>
    [FunctionName("BlobCleanupFunction")]
    [Obsolete]
    public static async void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
    {
        // Initialize Application Insights telemetry client.
        InitializeTelemetry(context);

        // Log information about function execution time.
        log.LogInformation($"BlobCleanupFunction executed at: {DateTime.Now}");

        // Load configuration settings from local.settings.json and environment variables.
        IConfigurationRoot config = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables()
          .Build();

        // Retrieve the Azure Blob Storage connection string and container name from configuration.
        string connectionString = config.GetValue<string>("AzureBlobStorage:blobConnectionString");
        string containerName = config.GetValue<string>("AzureBlobStorage:blobContainerName");

        // Create CloudStorageAccount and CloudBlobClient objects using the connection string.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);

        // Retrieve the list of blobs in the container with their creation dates.
        BlobResultSegment blobList = await container.ListBlobsSegmentedAsync(null, null);
        List<BlobMetadata> blobMetadataList = blobList.Results.OfType<CloudBlockBlob>().Select(b => new BlobMetadata
        {
            Blob = b,
            Created = b.Properties.Created.Value
        }).ToList();

        // Calculate the number of blobs to delete (retain the latest 10 blobs).
        int blobsToDeleteCount = blobMetadataList.Count - 10;
        IEnumerable<BlobMetadata> blobsToDelete = blobMetadataList.OrderBy(b => b.Created).Take(blobsToDeleteCount);

        // Delete the oldest blobs.
        foreach (BlobMetadata blobMetadata in blobsToDelete)
        {
            await blobMetadata.Blob.DeleteAsync();
            // Log deletion information for each blob.
            log.LogInformation($"Deleted blob: {blobMetadata.Blob.Name} - Created: {blobMetadata.Created}");
        }
    }

    /// <summary>
    /// Represents metadata about a blob including the CloudBlockBlob object and its creation date.
    /// </summary>
    public class BlobMetadata
    {
        public CloudBlockBlob Blob { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    /// <summary>
    /// Initialize the Application Insights telemetry client using the instrumentation key from configuration.
    /// </summary>
    /// <param name="context">The execution context containing information about the function execution environment.</param>
    
    [Obsolete]
    private static void InitializeTelemetry(ExecutionContext context)
    {
        // Load configuration settings from local.settings.json and environment variables.
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Retrieve the Application Insights instrumentation key from configuration.
        string instrumentationKey = config.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

        // Check if the telemetry client has already been initialized.
        if (telemetryClient == null)
        {
            // Throw an exception if the instrumentation key is not provided.
            if (string.IsNullOrEmpty(instrumentationKey))
            {
                throw new InvalidOperationException("APPINSIGHTS_INSTRUMENTATIONKEY environment variable is not set.");
            }

            // Create a new telemetry client with the provided instrumentation key.
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = $"InstrumentationKey={instrumentationKey}";
            telemetryClient = new TelemetryClient(configuration);
        }
    }
}
