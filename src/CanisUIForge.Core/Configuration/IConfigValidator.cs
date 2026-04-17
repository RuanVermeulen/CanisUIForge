namespace CanisUIForge.Core.Configuration;

public interface IConfigValidator
{
    ConfigValidationResult Validate(ForgeConfig config);
}
