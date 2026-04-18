namespace CanisUIForge.Blazor.Generators;

public class BlazorApiServiceGenerator : IBlazorApiServiceGenerator
{
    private readonly ApiServiceInterfaceGenerator _interfaceGenerator;
    private readonly ApiServiceImplementationGenerator _implementationGenerator;
    private readonly IFileWriter _fileWriter;

    public BlazorApiServiceGenerator(
        ApiServiceInterfaceGenerator interfaceGenerator,
        ApiServiceImplementationGenerator implementationGenerator,
        IFileWriter fileWriter)
    {
        _interfaceGenerator = interfaceGenerator ?? throw new ArgumentNullException(nameof(interfaceGenerator));
        _implementationGenerator = implementationGenerator ?? throw new ArgumentNullException(nameof(implementationGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string blazorProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Blazor");
        string servicesDirectory = Path.Combine(blazorProjectPath, "Services");
        _fileWriter.EnsureDirectoryExists(servicesDirectory);

        foreach (ResolvedResource resource in plan.Resources)
        {
            await _interfaceGenerator.GenerateAsync(plan, resource, blazorProjectPath);
            await _implementationGenerator.GenerateAsync(plan, resource, blazorProjectPath);
        }
    }
}
