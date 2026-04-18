namespace CanisUIForge.Blazor.Generators;

public interface IBlazorApiServiceGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
