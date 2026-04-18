namespace CanisUIForge.Generation.Validation;

public class PipelineValidator : IPipelineValidator
{
    public PipelineValidationResult ValidateContractsSource(ContractsConfig contractsConfig)
    {
        PipelineValidationResult result = new PipelineValidationResult();

        if (contractsConfig is null)
        {
            result.AddError("Contracts configuration is required.");
            return result;
        }

        switch (contractsConfig.Mode)
        {
            case ContractsMode.ProjectReference:
                ValidateProjectReference(contractsConfig, result);
                break;

            case ContractsMode.NuGetReference:
                ValidateNuGetReference(contractsConfig, result);
                break;
        }

        return result;
    }

    public PipelineValidationResult ValidateSchemaResolution(ApiDefinition apiDefinition, ITypeRegistry typeRegistry)
    {
        PipelineValidationResult result = new PipelineValidationResult();

        if (apiDefinition is null)
        {
            result.AddError("API definition is required for schema validation.");
            return result;
        }

        if (typeRegistry is null)
        {
            result.AddError("Type registry is required for schema validation.");
            return result;
        }

        List<string> schemaNames = CollectSchemaNames(apiDefinition);

        if (schemaNames.Count == 0)
        {
            result.AddWarning("No schema names found in API definition endpoints.");
            return result;
        }

        List<string> unresolvedSchemas = new List<string>();

        foreach (string schemaName in schemaNames)
        {
            if (!typeRegistry.Contains(schemaName))
            {
                unresolvedSchemas.Add(schemaName);
            }
        }

        if (unresolvedSchemas.Count > 0)
        {
            foreach (string unresolved in unresolvedSchemas)
            {
                result.AddWarning($"Schema '{unresolved}' could not be resolved to a CLR type.");
            }

            int resolvedCount = schemaNames.Count - unresolvedSchemas.Count;
            result.AddWarning(
                $"Resolved {resolvedCount} of {schemaNames.Count} schema(s). " +
                "Unresolved schemas will use 'object' as the fallback type.");
        }

        return result;
    }

    private static void ValidateProjectReference(ContractsConfig contractsConfig, PipelineValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(contractsConfig.ProjectPath))
        {
            result.AddError("Contracts project path is required for ProjectReference mode.");
            return;
        }

        if (!File.Exists(contractsConfig.ProjectPath))
        {
            result.AddError($"Contracts project not found at path: {contractsConfig.ProjectPath}");
        }
    }

    private static void ValidateNuGetReference(ContractsConfig contractsConfig, PipelineValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(contractsConfig.PackageId))
        {
            result.AddError("Contracts package ID is required for NuGetReference mode.");
            return;
        }

        if (string.IsNullOrWhiteSpace(contractsConfig.PackageVersion))
        {
            result.AddError("Contracts package version is required for NuGetReference mode.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(contractsConfig.LocalFeed) && !Directory.Exists(contractsConfig.LocalFeed))
        {
            result.AddWarning($"Local NuGet feed directory not found: {contractsConfig.LocalFeed}");
        }
    }

    private static List<string> CollectSchemaNames(ApiDefinition apiDefinition)
    {
        List<string> schemaNames = new List<string>();

        foreach (ResourceDefinition resource in apiDefinition.Resources)
        {
            foreach (EndpointDefinition endpoint in resource.Endpoints)
            {
                if (!string.IsNullOrWhiteSpace(endpoint.RequestSchemaName))
                {
                    schemaNames.Add(endpoint.RequestSchemaName);
                }

                if (!string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName))
                {
                    schemaNames.Add(endpoint.ResponseSchemaName);
                }
            }
        }

        return schemaNames
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
