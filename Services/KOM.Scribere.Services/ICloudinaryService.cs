using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KOM.Scribere.Services;

public interface ICloudinaryService
{
    Task<string> UploadMediaAsync(IFormFile media, string fileName, string folder);
}