using CosmosDbCrudByRP.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDbCrudByRP.Services
{
    public class CosmosClientWrapper : ICosmosClientWrapper
    {
        private readonly Container _container;

        private readonly IConfiguration _configuration;
        private CosmosClient _cosmosClient;

        public CosmosClientWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Container GetContainer(string databaseName, string containerName)
        {
            if (_cosmosClient == null)
            {
                _cosmosClient = new CosmosClient(_configuration.GetValue<string>("CosmosDb:AccountUri"),
                    _configuration.GetValue<string>("CosmosDb:PrimaryKey"));
            }

            Database database = _cosmosClient.GetDatabase(databaseName);
            return database.GetContainer(containerName);
        }

        public async Task<ItemResponse<EmployeeModel>> ReadItemAsync<T>(string id, PartitionKey partitionKey)
        {
            try
            {
                return await _container.ReadItemAsync<EmployeeModel>(id, partitionKey);
            }
            catch (CosmosException)
            {
                // Handle exceptions or rethrow if needed
                throw;
            }
        }
    }
}
