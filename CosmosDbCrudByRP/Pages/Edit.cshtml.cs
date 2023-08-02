using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbCrudByRP.Models;
using CosmosDbCrudByRP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbCrudByRP.Pages
{
    public class EditModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        [BindProperty]
        public EmployeeModel Employee { get; set; }
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public EditModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region EditOperation
        public async Task<IActionResult> OnGetAsync(string id,string partitionKey)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            Employee= await _employeeService.GetEmployeeAsync(id,partitionKey);

            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                await _employeeService.UpdateEmployeeAsync(Employee,Employee.id);
                TempData["Updated"] = "Record Updated Successfully";
                return RedirectToPage("/EmployeeList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the employee: " + ex.Message);
                return Page();
            }
        }
        #endregion

    }
}
