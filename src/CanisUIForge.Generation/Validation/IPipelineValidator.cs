namespace CanisUIForge.Generation.Validation;

public interface IPipelineValidator
{
    PipelineValidationResult ValidateContractsSource(ContractsConfig contractsConfig);

    PipelineValidationResult ValidateSchemaResolution(ApiDefinition apiDefinition, ITypeRegistry typeRegistry);
}
