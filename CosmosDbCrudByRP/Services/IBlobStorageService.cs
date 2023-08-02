using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace CosmosDbCrudByRP.Services
{
   public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
