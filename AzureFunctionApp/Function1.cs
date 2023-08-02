using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;


public static class Function1
{
    private static TelemetryClient telemetryClient;

    /// <summary>
    /// Function triggered by an HTTP GET request to retrieve data from Cosmos DB.
    /// </summary>// Function triggered by an HTTP GET request
    [FunctionName("Function1")]
    [Obsolete]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        // Initialize Application Insights telemetry
        InitializeTelemetry();

        log.LogInformation("C# HTTP trigger function processed a request.");

        // Get the connection string for Cosmos DB from environment variables
        string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
        // Extract endpoint and key from the connection string
        string cosmosDbEndpoint = ExtractCosmosDbEndpointFromConnectionString(connectionString);
        string cosmosDbKey = ExtractCosmosDbKeyFromConnectionString(connectionString);

        // Define the Cosmos DB database and container names and the query
        string databaseName = "EmployeeManagementDB";
        string containerName = "Employees";
        string query = "SELECT * FROM c";


        // Create a new DocumentClient instance to interact with Cosmos DB
        using (var client = new DocumentClient(new Uri(cosmosDbEndpoint), cosmosDbKey))
        {
            // Configure feed options to enable cross-partition queries
            FeedOptions requestOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = true
            };

            // Create a list to store the retrieved documents
            var documents = new List<dynamic>();
            // Execute the document query and iterate through the results
            var documentQuery = client.CreateDocumentQuery<dynamic>(
                UriFactory.CreateDocumentCollectionUri(databaseName, containerName),
                query,
                requestOptions).AsDocumentQuery();

            while (documentQuery.HasMoreResults)
            {
                var feedResponse = documentQuery.ExecuteNextAsync().Result;
                documents.AddRange(feedResponse);
            }


            // Create a dependency telemetry for tracking the Cosmos DB call
            var dependencyTelemetry = new DependencyTelemetry()
            {
                Name = "CosmosDB Call",
                Target = "cosmosDbEndpoint",
                Type = "CosmosDB",
                Data = query,
                Success = true, 
                ResultCode = "200", 
                Properties =
                {
                    ["DatabaseName"] = databaseName,
                    ["CollectionName"] = containerName,
                    ["Query"] = query,
                    ["RequestCharge"] = GetRequestCharge(documentQuery)
                }
            };

            // Track the Cosmos DB dependency
            telemetryClient.TrackDependency(dependencyTelemetry);

            // Return the retrieved documents as an HTTP response
            if (documents.Count > 0)
            {
                return new OkObjectResult(documents);
            }
            else
            { 
                return new NotFoundResult();
            }
        }
    }

   
    [Obsolete]
    /// <summary>
    /// Initializes the TelemetryClient if not already initialized.
    /// </summary>
    private static void InitializeTelemetry()
    {
        if (telemetryClient == null)
        {
            telemetryClient = new TelemetryClient();
        }
    }

    /// <summary>
    /// Helper method to extract the Cosmos DB endpoint from the connection string.
    /// </summary>
    private static string ExtractCosmosDbEndpointFromConnectionString(string connectionString)
    {
        const string accountEndpointKey = "AccountEndpoint=";

  
        int endpointStartIndex = connectionString.IndexOf(accountEndpointKey);
        if (endpointStartIndex < 0)
        {
            throw new ArgumentException("Invalid Cosmos DB connection string. Missing AccountEndpoint.");
        }

        int endpointEndIndex = connectionString.IndexOf(';', endpointStartIndex);
        if (endpointEndIndex < 0)
        {
            endpointEndIndex = connectionString.Length;
        }

        int endpointLength = endpointEndIndex - (endpointStartIndex + accountEndpointKey.Length);
        return connectionString.Substring(endpointStartIndex + accountEndpointKey.Length, endpointLength);
    }

    /// <summary>
    /// Helper method to extract the Cosmos DB key from the connection string.
    /// </summary>
    private static string ExtractCosmosDbKeyFromConnectionString(string connectionString)
    {
        const string accountKeyKey = "AccountKey=";
        int keyStartIndex = connectionString.IndexOf(accountKeyKey);
        if (keyStartIndex < 0)
        {
            throw new ArgumentException("Invalid Cosmos DB connection string. Missing AccountKey.");
        }

        int keyEndIndex = connectionString.IndexOf(';', keyStartIndex);
        if (keyEndIndex < 0)
        {
            keyEndIndex = connectionString.Length;
        }

        int keyLength = keyEndIndex - (keyStartIndex + accountKeyKey.Length);
        return connectionString.Substring(keyStartIndex + accountKeyKey.Length, keyLength);
    }

    /// <summary>
    /// Helper method to get the request charge from the document query response.
    /// </summary>
    private static string GetRequestCharge(IDocumentQuery<dynamic> documentQuery)
    {
        var feedResponse = documentQuery.ExecuteNextAsync().Result;
        return feedResponse.RequestCharge.ToString("0.00");
    }
}
