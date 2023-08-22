using CosmosDbCrud_DAL.Model;
using CosmosDbCrud_DAL.Models;
using CosmosUnitTestProject.UnitTest;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDbCrudByRP.Tests
{
    [TestClass]
    public class EmployeeServiceTest
    {
        private EmployeeService _employeeService;
        private IConfiguration _config;

        [TestInitialize]
        public void TestInitialize()
        {
            // Read the configuration from local.json
            TestConfiguration.GetLocalConfiguration();
            _config = TestConfiguration.GetActualConfiguration();
            _employeeService = new EmployeeService(_config);
        }

        [TestMethod]
        public async Task GetEmployeeAsyncBy_Should_ReturnListOfEmployee()
        {
            // Arrange
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

            // Act
            List<EmployeeModel> resultList = (List<EmployeeModel>)await _employeeService.GetEmployeesAsync();

            // Assert
            Assert.IsNotNull(resultList);
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
            // Arrange
            string employeeId = "2";
            string partitionKey = "2";
            EmployeeModel expectedEmployee = new() { id = employeeId, Name = "Navya" };

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

