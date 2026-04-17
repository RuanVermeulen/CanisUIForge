using Microsoft.OpenApi.Models;

namespace CanisUIForge.OpenApi.Loading;

public interface ISwaggerLoader
{
    Task<OpenApiDocument> LoadAsync(string source);
}
