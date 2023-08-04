using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CosmosDbCrud_DAL.Implementations
{
   public static class testConfigurationsService
    {

        public static IConfiguration GetConfiguration(Dictionary<string, string> configurationValues)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            return configuration;
        }
        public static IConfiguration GetLocalConfig()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json")
                .Build();

            return configuration;
        }

        public static IConfiguration GetActualConfig()
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            return config;
        }
    }
}
