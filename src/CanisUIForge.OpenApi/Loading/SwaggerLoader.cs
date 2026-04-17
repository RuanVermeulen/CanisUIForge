using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace CanisUIForge.OpenApi.Loading;

public class SwaggerLoader : ISwaggerLoader
{
    public async Task<OpenApiDocument> LoadAsync(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Swagger source must not be null or empty.", nameof(source));
        }

        Stream stream = await OpenSourceStreamAsync(source);

        try
        {
            OpenApiStreamReader reader = new OpenApiStreamReader();
            OpenApiDocument document = reader.Read(stream, out OpenApiDiagnostic diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                List<string> errorMessages = diagnostic.Errors
                    .Select(error => error.Message)
                    .ToList();

                throw new InvalidOperationException(
                    $"Failed to parse Swagger document. Errors: {string.Join("; ", errorMessages)}");
            }

            return document;
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }

    private static async Task<Stream> OpenSourceStreamAsync(string source)
    {
        if (Uri.TryCreate(source, UriKind.Absolute, out Uri? uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return await LoadFromUrlAsync(uri);
        }

        return LoadFromFile(source);
    }

    private static async Task<Stream> LoadFromUrlAsync(Uri uri)
    {
        HttpClient httpClient = new HttpClient();

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            MemoryStream memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    private static Stream LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Swagger file not found: {filePath}", filePath);
        }

        return File.OpenRead(filePath);
    }
}
