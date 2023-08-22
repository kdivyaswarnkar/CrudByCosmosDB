using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AzureFunctionApp
{
    /// <summary>
    /// Azure Function that retrieves data from Cosmos DB using an HTTP Trigger.
    /// </summary>
    public static class GettingDataByHttpTrigger
    {

        /// <summary>
        /// Function triggered by an HTTP GET request to retrieve data from Cosmos DB.
        /// </summary>
        /// <param name="req">The incoming HTTP request.</param>
        /// <param name="documents">The collection of documents retrieved from Cosmos DB based on the specified SQL query.</param>
        /// <param name="log">The logger instance to log information.</param>
        /// <returns>An IActionResult containing the retrieved documents.</returns>
        [FunctionName("GettingDataByHttpTrigger")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
            databaseName: "EmployeeManagementDB",
            collectionName: "Employees",
            ConnectionStringSetting = "CosmosDBConnection",
            SqlQuery = "SELECT * FROM c")] IEnumerable<dynamic> documents,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Return the retrieved documents as an HTTP response
            return new OkObjectResult(documents);
        }
    }
    

}
