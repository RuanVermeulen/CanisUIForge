using CanisUIForge.Generation.Models;

namespace CanisUIForge.Blazor.Generators;

public interface IBlazorFoundationGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
