namespace CanisUIForge.Core.Configuration;

public class JsonConfigLoader : IConfigLoader
{
    private static readonly JsonSerializerOptions DeserializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public async Task<ForgeConfig> LoadAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {filePath}", filePath);
        }

        using FileStream stream = File.OpenRead(filePath);
        ForgeConfig? config = await JsonSerializer.DeserializeAsync<ForgeConfig>(stream, DeserializerOptions);

        if (config is null)
        {
            throw new InvalidOperationException($"Failed to deserialize configuration from: {filePath}");
        }

        return config;
    }
}
