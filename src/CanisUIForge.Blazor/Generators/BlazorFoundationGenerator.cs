using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;

namespace CanisUIForge.Blazor.Generators;

public class BlazorFoundationGenerator : IBlazorFoundationGenerator
{
    private readonly BlazorProjectGenerator _projectGenerator;
    private readonly BlazorBootstrapGenerator _bootstrapGenerator;
    private readonly LayoutGenerator _layoutGenerator;
    private readonly NavigationGenerator _navigationGenerator;
    private readonly HomePageGenerator _homePageGenerator;
    private readonly IFileWriter _fileWriter;

    public BlazorFoundationGenerator(
        BlazorProjectGenerator projectGenerator,
        BlazorBootstrapGenerator bootstrapGenerator,
        LayoutGenerator layoutGenerator,
        NavigationGenerator navigationGenerator,
        HomePageGenerator homePageGenerator,
        IFileWriter fileWriter)
    {
        _projectGenerator = projectGenerator ?? throw new ArgumentNullException(nameof(projectGenerator));
        _bootstrapGenerator = bootstrapGenerator ?? throw new ArgumentNullException(nameof(bootstrapGenerator));
        _layoutGenerator = layoutGenerator ?? throw new ArgumentNullException(nameof(layoutGenerator));
        _navigationGenerator = navigationGenerator ?? throw new ArgumentNullException(nameof(navigationGenerator));
        _homePageGenerator = homePageGenerator ?? throw new ArgumentNullException(nameof(homePageGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string blazorProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Blazor");

        _fileWriter.EnsureDirectoryExists(blazorProjectPath);
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "Layout"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "Pages"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "Components"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "wwwroot", "css"));

        await _projectGenerator.GenerateAsync(plan, blazorProjectPath);
        await _bootstrapGenerator.GenerateAsync(plan, blazorProjectPath);
        await _layoutGenerator.GenerateAsync(plan, blazorProjectPath);
        await _navigationGenerator.GenerateAsync(plan, blazorProjectPath);
        await _homePageGenerator.GenerateAsync(plan, blazorProjectPath);
    }
}
