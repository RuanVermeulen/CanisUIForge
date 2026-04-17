namespace CanisUIForge.Contracts.Registry;

public interface ITypeRegistry
{
    void Register(string schemaName, Type type);

    Type? Resolve(string schemaName);

    bool Contains(string schemaName);

    IReadOnlyDictionary<string, Type> GetAll();
}
