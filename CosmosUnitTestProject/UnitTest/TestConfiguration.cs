using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosUnitTestProject.UnitTest
{

    public static class TestConfiguration
    {
        public static IConfiguration GetMockConfiguration(Dictionary<string, string> configurationValues)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                          .AddInMemoryCollection(configurationValues)
                          .Build();
            return configuration;
        }

    }
}

