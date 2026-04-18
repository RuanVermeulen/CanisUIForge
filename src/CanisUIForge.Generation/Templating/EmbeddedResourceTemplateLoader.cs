namespace CanisUIForge.Generation.Templating;

public class EmbeddedResourceTemplateLoader : ITemplateLoader
{
    private readonly Assembly _assembly;
    private readonly string _baseResourcePath;
    private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

    public EmbeddedResourceTemplateLoader(Assembly assembly, string baseResourcePath)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        _baseResourcePath = baseResourcePath ?? throw new ArgumentNullException(nameof(baseResourcePath));
    }

    public string Load(string templatePath)
    {
        if (string.IsNullOrWhiteSpace(templatePath))
        {
            throw new ArgumentException("Template path must not be null or empty.", nameof(templatePath));
        }

        if (_cache.TryGetValue(templatePath, out string? cached))
        {
            return cached;
        }

        string resourceName = $"{_baseResourcePath}.{templatePath.Replace('/', '.')}.sbn";
        using Stream? stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Embedded template resource not found: {resourceName}. " +
                $"Available resources: {string.Join(", ", _assembly.GetManifestResourceNames())}");
        }

        using StreamReader reader = new StreamReader(stream);
        string content = reader.ReadToEnd();
        _cache[templatePath] = content;
        return content;
    }
}
