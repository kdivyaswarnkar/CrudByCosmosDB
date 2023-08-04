using CosmosDbCrud_DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDbCrud_DAL.Model
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeModel>> GetEmployeesAsync();
        Task<EmployeeModel> GetEmployeeAsync(string id, string partitionKey);
        Task AddEmployeeAsync(EmployeeModel employee);
        Task UpdateEmployeeAsync(EmployeeModel employee, string partitionKey);
        Task DeleteEmployeeAsync(string id, string partitionKey);
    }
}
