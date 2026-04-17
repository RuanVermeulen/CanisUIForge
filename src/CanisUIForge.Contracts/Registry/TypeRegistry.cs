namespace CanisUIForge.Contracts.Registry;

public class TypeRegistry : ITypeRegistry
{
    private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

    public void Register(string schemaName, Type type)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            throw new ArgumentException("Schema name must not be null or empty.", nameof(schemaName));
        }

        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        _types[schemaName] = type;
    }

    public Type? Resolve(string schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            return null;
        }

        _types.TryGetValue(schemaName, out Type? resolvedType);
        return resolvedType;
    }

    public bool Contains(string schemaName)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            return false;
        }

        return _types.ContainsKey(schemaName);
    }

    public IReadOnlyDictionary<string, Type> GetAll()
    {
        return _types;
    }
}
