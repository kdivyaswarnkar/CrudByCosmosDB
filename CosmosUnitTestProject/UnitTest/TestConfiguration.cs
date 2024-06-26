﻿using Microsoft.Extensions.Configuration;
using System.Collections.Generic;


namespace CosmosUnitTestProject.UnitTest
{

    public static class TestConfiguration
    {
        public static IConfiguration GetMockConfiguration(Dictionary<string, string> configurationValues)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            return configuration;
        }

        public static IConfiguration GetLocalConfiguration()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json")
                .Build();

            return configuration;
        }

        public static IConfiguration GetActualConfiguration()
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
            return config;
        }

    }
}

