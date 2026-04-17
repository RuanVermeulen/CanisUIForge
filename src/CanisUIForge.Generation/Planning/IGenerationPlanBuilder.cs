using CanisUIForge.Contracts.Registry;
using CanisUIForge.Core.Configuration;
using CanisUIForge.Generation.Models;
using CanisUIForge.OpenApi.Models;

namespace CanisUIForge.Generation.Planning;

public interface IGenerationPlanBuilder
{
    GenerationPlan Build(ForgeConfig config, ApiDefinition apiDefinition, ITypeRegistry typeRegistry);
}
