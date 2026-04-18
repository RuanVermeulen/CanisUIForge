namespace CanisUIForge.Testing.Generators;

public interface IAppiumTestGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
