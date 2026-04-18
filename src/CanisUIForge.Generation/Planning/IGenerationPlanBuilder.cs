namespace CanisUIForge.Generation.Planning;

public interface IGenerationPlanBuilder
{
    GenerationPlan Build(ForgeConfig config, ApiDefinition apiDefinition, ITypeRegistry typeRegistry);
}
