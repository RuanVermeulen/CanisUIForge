namespace CanisUIForge.OpenApi.Loading;

public interface ISwaggerLoader
{
    Task<OpenApiDocument> LoadAsync(string source);
}
