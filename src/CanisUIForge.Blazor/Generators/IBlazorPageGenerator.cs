using CanisUIForge.Generation.Models;

namespace CanisUIForge.Blazor.Generators;

public interface IBlazorPageGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
