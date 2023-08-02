using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;


namespace CosmosDbCrudByRP.Configurations
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Cosmos DB container to the service collection using the provided configuration section.
        /// </summary>
        /// <param name="services">The service collection to add the Cosmos DB container to.</param>
        /// <param name="configurationSection">The configuration section containing the required settings for Cosmos DB.</param>
        /// <returns>The updated service collection.</returns>
        #region ConfigurationDataProviders
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            // Register Cosmos DB Container
            services.AddSingleton(serviceProvider =>
            {
                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                string cosmosDbAccountUri = configurationSection.GetValue<string>("AccountUri");
                string cosmosDbAccountKey = configurationSection.GetValue<string>("PrimaryKey");
                string cosmosDbName = configurationSection.GetValue<string>("DatabaseName");
                string cosmosDbContainerName = configurationSection.GetValue<string>("ContainerName");

                CosmosClientBuilder cosmosClientBuilder = new(cosmosDbAccountUri, cosmosDbAccountKey);
                CosmosClient cosmosClient = cosmosClientBuilder.WithConnectionModeDirect().Build();

                // Get the reference to the Cosmos DB container.
                Container cosmosDbContainer = cosmosClient.GetContainer(cosmosDbName, cosmosDbContainerName);

                return cosmosDbContainer;
            });

            return services;
        }

        #endregion
    }
}
