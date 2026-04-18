namespace CanisUIForge.Testing.Generators;

public interface IPlaywrightTestGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
