namespace CanisUIForge.Blazor.Generators;

public interface IBlazorComponentsGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
