namespace CanisUIForge.OpenApi.Scanning;

public interface IOpenApiScanner
{
    Task<ApiDefinition> ScanAsync(string swaggerSource);
}
