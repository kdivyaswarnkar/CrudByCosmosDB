using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbCrudByRP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbCrudByRP.Pages
{
    public class UploadModel : PageModel
    {
        private readonly IBlobStorageService _blobStorageService;

        public UploadModel(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public IFormFile Image { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Image != null && Image.Length > 0)
            {
                var imageUrl = await _blobStorageService.UploadFileAsync(Image);

                TempData["UploadSuccess"] = "Image uploaded successfully.";
            }

            return RedirectToPage("/Upload");
        }
    }
}
