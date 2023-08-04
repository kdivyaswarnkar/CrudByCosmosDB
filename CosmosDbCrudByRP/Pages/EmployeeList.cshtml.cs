using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbCrud_DAL.Model;
using CosmosDbCrud_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbCrudByRP.Pages
{  
    public class EmployeeListModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        [BindProperty]
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EmployeeModel> Employees { get; set; }

        public EmployeeListModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region GetEmployeeList
        public async Task OnGetAsync()
        {
            try
            {
                Task<IEnumerable<EmployeeModel>> employeesTask = _employeeService.GetEmployeesAsync();
                IEnumerable<EmployeeModel> employees = await employeesTask;
                ViewData["Employees"] = employees;
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "An error occurred while retrieving the employees." + ex;
            }
        }

        public async Task<IActionResult> OnPostAsync(string command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (command.ToLower())
                    {
                        case "add":
                            await _employeeService.AddEmployeeAsync(Employee);
                            break;
                    }
                    TempData["Added"] = "Record Added Successfully";
                    return RedirectToPage("/EmployeeList");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing the request." + ex);
            }

            Employees = await _employeeService.GetEmployeesAsync();
            return Page();
        }
        #endregion
    }
}
