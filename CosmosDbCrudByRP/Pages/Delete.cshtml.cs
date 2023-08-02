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
    public class DeleteModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        [BindProperty]
        public string id { get; set; }

        [BindProperty]
        public string PartitionKey { get; set; }
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public DeleteModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region DeleteOperation
        public IActionResult OnGetAsync(string id, string partitionKey)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(partitionKey))
                {
                    return NotFound();
                }

                this.id = id;
                PartitionKey = partitionKey;

                return Page();
            }
            catch (Exception ex)
            {
                return Content("An error occurred: " + ex.Message);
            }
        }

        //delete post
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(PartitionKey))
            {
                return NotFound();
            }

            try
            {
                await _employeeService.DeleteEmployeeAsync(id, PartitionKey);
                return RedirectToPage("/EmployeeList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the employee: " + ex.Message);
                return Page();
            }
        }
        #endregion

    }
}
