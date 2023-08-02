using CosmosDbCrudByRP.Models;
using CosmosDbCrudByRP.Services;
using CosmosUnitTestProject.UnitTest;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDbCrudByRP.Tests
{

    [TestClass]
    public class EmployeeServiceTest
    {
        public readonly EmployeeService _employeeService;
        private IConfiguration _configuration;
        private ICosmosClientWrapper _cosmosClientWrapper;

        [TestMethod]
        public async Task GetEmployeeAsyncBy_Should_ReturnListOfEmployee()
        {
            // Arrange
            Mock<ICosmosClientWrapper> mockCosmosClientWrapper = new();
            Mock<IConfiguration> mockConfiguration = new();
            Mock<Container> mockCont = new();

            Dictionary<string, string> configurationValues = new()
            {
                { "CosmosDb:AccountUri", "https://mock-cosmos-account.documents.azure.com:443/" },
                { "CosmosDb:PrimaryKey", "SGcGKqZGhXJe8SqRZwNRt71lpqrfvhZe5BFD1ZvRoCf52CmJIVts9ozMKlbMdx0eEZk8EJiiOQibACDbKKukFA==" },
                { "CosmosDb:DatabaseName", "MockDatabase" },
                { "CosmosDb:ContainerName", "MockContainer" }
            };

            // Get the mock IConfiguration using the TestConfiguration class
            IConfiguration mockConfig = TestConfiguration.GetMockConfiguration(configurationValues);

            // Create the CosmosClientWrapper using the mocked IConfiguration
            CosmosClientWrapper cosmosClientWrapper = new(mockConfig);

            // Create a list of mocked EmployeeModel objects
            List<EmployeeModel> expectedEmployees = new()
            {
                new EmployeeModel { id = "1", Name = "Divya" },
                new EmployeeModel { id = "2", Name = "Navya" },
                new EmployeeModel { id = "3", Name = "test3" },
                new EmployeeModel { id = "4", Name = "test4" },
                new EmployeeModel { id = "5", Name = "test5" },
                new EmployeeModel { id = "6", Name = "test6" },
                new EmployeeModel { id = "7", Name = "test7" },
                new EmployeeModel { id = "8", Name = "test8" },
                new EmployeeModel { id = "9", Name = "test9" },
            };

            // Setup the mock behavior for GetContainer to return the mockContainer
            mockCosmosClientWrapper.Setup(cw => cw.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(mockCont.Object);

            //  EmployeeService employeeService = new(mockConfig, mockCosmosClientWrapper.Object);
            // Load the actual configuration (from appsettings.json or other sources)
            IConfiguration _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Replace this with your actual configuration source
                .Build();

            // Create the CosmosClientWrapper using the actual IConfiguration
            CosmosClientWrapper _cosmosClientWrapper = new(_configuration);

            // Initialize the EmployeeService using the actual IConfiguration and the real CosmosClientWrapper
            EmployeeService _employeeService = new(_configuration, _cosmosClientWrapper);

            // Act
            List<EmployeeModel> resultList = (List<EmployeeModel>)await _employeeService.GetEmployeesAsync();

            // Assert
            Assert.IsNotNull(resultList);
            //Assert.AreEqual(expectedEmployees.Count, resultList.Count);
            Assert.AreEqual(expectedEmployees.Count, resultList.Count);
            for (int i = 0; i < expectedEmployees.Count; i++)
            {
                Assert.AreNotEqual(expectedEmployees[i].id, resultList[i].id);
                Assert.AreNotEqual(expectedEmployees[i].Name, resultList[i].Name);
            }
            Console.WriteLine("Test executed successfully.");
        }


        [TestMethod]
        public async Task GetEmployeeAsyncBy_Should_Return_IdAndPartitionKey()
        {
            Mock<ICosmosClientWrapper> mockCosmosClientWrapper = new();
            Mock<IConfiguration> mockConfiguration = new();
            Mock<Container> mockCont = new();

            Dictionary<string, string> configurationValues = new()
            {
                { "CosmosDb:AccountUri", "https://mock-cosmos-account.documents.azure.com:443/" },
                { "CosmosDb:PrimaryKey", "SGcGKqZGhXJe8SqRZwNRt71lpqrfvhZe5BFD1ZvRoCf52CmJIVts9ozMKlbMdx0eEZk8EJiiOQibACDbKKukFA==" },
                { "CosmosDb:DatabaseName", "MockDatabase" },
                { "CosmosDb:ContainerName", "MockContainer" }
            };

            // Get the mock IConfiguration using the TestConfiguration class
            IConfiguration mockConfig = TestConfiguration.GetMockConfiguration(configurationValues);

            // Create the CosmosClientWrapper using the mocked IConfiguration
            CosmosClientWrapper cosmosClientWrapper = new(mockConfig);

            // Arrange
            string employeeId = "2";
            string partitionKey = "2";
            EmployeeModel expectedEmployee = new() { id = employeeId, Name = "Navya" };

            // Create a mocked ItemResponse using Moq
            Mock<ItemResponse<EmployeeModel>> itemResponse = new();
            itemResponse.SetupGet(r => r.Resource).Returns(expectedEmployee);
            itemResponse.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);

            // Setup the mock behavior for GetContainer to return the mockContainer
            mockCosmosClientWrapper.Setup(cw => cw.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(mockCont.Object);

            // Setup the mock behavior for ReadItemAsync to return the mock ItemResponse directly
            mockCont.Setup(c => c.ReadItemAsync<EmployeeModel>(
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                    )).ReturnsAsync(itemResponse.Object);

            //  EmployeeService employeeService = new(mockConfig, mockCosmosClientWrapper.Object);
            // Load the actual configuration (from appsettings.json or other sources)
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Replace this with your actual configuration source
                .Build();

            // Create the CosmosClientWrapper using the actual IConfiguration
            _cosmosClientWrapper = new CosmosClientWrapper(_configuration);

            // Initialize the EmployeeService using the actual IConfiguration and the real CosmosClientWrapper
            EmployeeService _employeeService = new(_configuration, _cosmosClientWrapper);

            // Act
            EmployeeModel result = await _employeeService.GetEmployeeAsync(employeeId, partitionKey);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedEmployee.id, result.id);
            Assert.AreEqual(expectedEmployee.Name, result.Name);
            Console.WriteLine("Test executed successfully.");

        }    
    }
}
