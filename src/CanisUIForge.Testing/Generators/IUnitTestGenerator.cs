namespace CanisUIForge.Testing.Generators;

public interface IUnitTestGenerator
{
    Task GenerateAsync(GenerationPlan plan);
}
