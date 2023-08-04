using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CosmosDbCrud_DAL.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
