using CosmosDbCrud_DAL.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CosmosDbCrud_DAL.Model
{
    public class EmployeeService : IEmployeeService
    {
        private readonly Container _container;

        public EmployeeService(IConfiguration configu)
        {
            string accountUri = configu["Values:AccountUri"];
            string primaryKey = configu["Values:PrimaryKey"];
            string databaseName = configu["Values:DatabaseName"];
            string containerName = configu["Values:ContainerName"];

            CosmosClient cosmosClient = new CosmosClient(accountUri, primaryKey);
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        // Get All Employees 
        #region GetAllEmployees
        public async Task<IEnumerable<EmployeeModel>> GetEmployeesAsync()
        {
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");
                FeedIterator<EmployeeModel> queryResultSetIterator = _container.GetItemQueryIterator<EmployeeModel>(queryDefinition);
                List<EmployeeModel> results = new List<EmployeeModel>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<EmployeeModel> response = await queryResultSetIterator.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (CosmosException ex)
            {
                throw new Exception("An error occurred while querying employees from Cosmos DB.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving employees.", ex);
            }
        }
        #endregion

        // Get Employee By Id And PartitionKey=id
        #region GetEmployeeById
        public async Task<EmployeeModel> GetEmployeeAsync(string id, string partitionKey)
        {
            try
            {
                ItemResponse<EmployeeModel> response = await _container.ReadItemAsync<EmployeeModel>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                throw new Exception("An error occurred while querying employees from Cosmos DB.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving employees.", ex);
            }
        }
        #endregion


        // Add Employee Details
        #region AddEmployee
        public async Task AddEmployeeAsync(EmployeeModel employee)
        {
            try
            {
                employee.id = Guid.NewGuid().ToString();
                await _container.CreateItemAsync(employee, new PartitionKey(employee.id));
            }
            catch (CosmosException ex)
            {
                throw new Exception("An error occurred while adding the employee to Cosmos DB.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the employee.", ex);
            }
        }
        #endregion


        // Update Employee Details
        #region UpdateEmployee
        public async Task UpdateEmployeeAsync(EmployeeModel employee, string partitionKey)
        {
            try
            {
                await _container.UpsertItemAsync(employee, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex)
            {
                throw new Exception("An error occurred while updating the employee in Cosmos DB.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the employee.", ex);
            }
        }
        #endregion


        // Delete Employee by Id / Partitionkey
        #region DeleteEmployee
        public async Task DeleteEmployeeAsync(string id, string partitionKey)
        {
            try
            {
                await _container.DeleteItemAsync<EmployeeModel>(id, new PartitionKey(partitionKey));
            }
            catch (CosmosException ex)
            {
                throw new Exception("An error occurred while deleting the employee from Cosmos DB.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the employee.", ex);
            }
        }
        #endregion
    }
}
