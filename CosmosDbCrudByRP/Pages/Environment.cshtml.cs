using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace CosmosDbCrudByRP.Pages
{
    public class EnvironmentModel : PageModel
    {
        private readonly IConfiguration _config;
        public EnvironmentModel(IConfiguration config)
        {
            _config = config;
        }
        public string EnvironmentName { get; set; }
        public void OnGet()
        {
            EnvironmentName = _config["Environment"];
        }
    }
}
