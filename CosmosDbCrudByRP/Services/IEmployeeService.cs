using CosmosDbCrudByRP.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDbCrudByRP.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeModel>> GetEmployeesAsync();
        Task<EmployeeModel> GetEmployeeAsync(string id, string partitionKey);
        Task AddEmployeeAsync(EmployeeModel employee);
        Task UpdateEmployeeAsync(EmployeeModel employee, string partitionKey);
        Task DeleteEmployeeAsync(string id,string partitionKey);
    }

    // Interface for CosmosClient
    public interface ICosmosClientWrapper
    {
        Container GetContainer(string databaseName, string containerName);
        Task<ItemResponse<EmployeeModel>> ReadItemAsync<T>(string id, PartitionKey partitionKey);
    }

   

}
