namespace CanisUIForge.Core.Configuration;

public class ConfigValidator : IConfigValidator
{
    public ConfigValidationResult Validate(ForgeConfig config)
    {
        ConfigValidationResult result = new ConfigValidationResult();

        ValidateSolutionName(config, result);
        ValidateTargets(config, result);
        ValidateSwaggerSource(config, result);
        ValidateContracts(config, result);
        ValidateControllers(config, result);

        return result;
    }

    private static void ValidateSolutionName(ForgeConfig config, ConfigValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(config.SolutionName))
        {
            result.AddError("SolutionName is required.");
        }
    }

    private static void ValidateTargets(ForgeConfig config, ConfigValidationResult result)
    {
        if (config.Targets.Count == 0)
        {
            result.AddError("At least one target platform must be specified.");
        }
    }

    private static void ValidateSwaggerSource(ForgeConfig config, ConfigValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(config.SwaggerSource))
        {
            result.AddError("SwaggerSource is required.");
        }
    }

    private static void ValidateContracts(ForgeConfig config, ConfigValidationResult result)
    {
        if (config.Contracts is null)
        {
            result.AddError("Contracts configuration is required.");
            return;
        }

        switch (config.Contracts.Mode)
        {
            case ContractsMode.ProjectReference:
                if (string.IsNullOrWhiteSpace(config.Contracts.ProjectPath))
                {
                    result.AddError("Contracts ProjectPath is required when using ProjectReference mode.");
                }
                break;

            case ContractsMode.NuGetReference:
                if (string.IsNullOrWhiteSpace(config.Contracts.PackageId))
                {
                    result.AddError("Contracts PackageId is required when using NuGetReference mode.");
                }
                if (string.IsNullOrWhiteSpace(config.Contracts.PackageVersion))
                {
                    result.AddError("Contracts PackageVersion is required when using NuGetReference mode.");
                }
                break;

            default:
                result.AddError($"Unknown ContractsMode: {config.Contracts.Mode}.");
                break;
        }
    }

    private static void ValidateControllers(ForgeConfig config, ConfigValidationResult result)
    {
        for (int index = 0; index < config.Controllers.Count; index++)
        {
            ControllerConfig controller = config.Controllers[index];

            if (string.IsNullOrWhiteSpace(controller.Name))
            {
                result.AddError($"Controller at index {index} must have a Name.");
            }
        }
    }
}
