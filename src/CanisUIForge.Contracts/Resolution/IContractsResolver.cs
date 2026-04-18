namespace CanisUIForge.Contracts.Resolution;

public interface IContractsResolver
{
    Task<ITypeRegistry> ResolveAsync(ContractsConfig contractsConfig);

    void ValidateSchemaNames(ITypeRegistry typeRegistry, IReadOnlyCollection<string> schemaNames);
}
