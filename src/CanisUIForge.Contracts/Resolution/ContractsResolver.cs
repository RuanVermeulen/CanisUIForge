using System.Reflection;
using CanisUIForge.Contracts.Loading;
using CanisUIForge.Contracts.Mapping;
using CanisUIForge.Contracts.Registry;
using CanisUIForge.Core.Configuration;
using CanisUIForge.Core.Enums;

namespace CanisUIForge.Contracts.Resolution;

public class ContractsResolver : IContractsResolver
{
    private readonly ISchemaTypeMapper _schemaTypeMapper;

    public ContractsResolver(ISchemaTypeMapper schemaTypeMapper)
    {
        _schemaTypeMapper = schemaTypeMapper ?? throw new ArgumentNullException(nameof(schemaTypeMapper));
    }

    public async Task<ITypeRegistry> ResolveAsync(ContractsConfig contractsConfig)
    {
        if (contractsConfig is null)
        {
            throw new ArgumentNullException(nameof(contractsConfig));
        }

        IAssemblyLoader assemblyLoader = CreateAssemblyLoader(contractsConfig);
        Assembly assembly = await assemblyLoader.LoadAsync();

        TypeRegistry typeRegistry = new TypeRegistry();
        _schemaTypeMapper.MapTypes(assembly, typeRegistry);

        return typeRegistry;
    }

    public void ValidateSchemaNames(ITypeRegistry typeRegistry, IReadOnlyCollection<string> schemaNames)
    {
        if (typeRegistry is null)
        {
            throw new ArgumentNullException(nameof(typeRegistry));
        }

        if (schemaNames is null)
        {
            throw new ArgumentNullException(nameof(schemaNames));
        }

        List<string> unresolvedSchemas = new List<string>();

        foreach (string schemaName in schemaNames)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
            {
                continue;
            }

            if (!typeRegistry.Contains(schemaName))
            {
                unresolvedSchemas.Add(schemaName);
            }
        }

        if (unresolvedSchemas.Count > 0)
        {
            throw new InvalidOperationException(
                $"The following schema names could not be resolved to CLR types: {string.Join(", ", unresolvedSchemas)}. " +
                "Ensure the contracts assembly contains matching types.");
        }
    }

    private static IAssemblyLoader CreateAssemblyLoader(ContractsConfig contractsConfig)
    {
        return contractsConfig.Mode switch
        {
            ContractsMode.ProjectReference => new ProjectReferenceLoader(contractsConfig.ProjectPath),
            ContractsMode.NuGetReference => new NuGetReferenceLoader(
                contractsConfig.PackageId,
                contractsConfig.PackageVersion,
                contractsConfig.LocalFeed),
            _ => throw new InvalidOperationException($"Unknown contracts mode: {contractsConfig.Mode}")
        };
    }
}
