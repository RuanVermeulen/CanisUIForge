using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class BlazorBootstrapGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public BlazorBootstrapGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string serviceRegistrations = ApiServiceGenerationHelper.BuildServiceRegistrations(
            plan.Resources, plan.NamespaceRoot);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "SolutionName", plan.SolutionName },
            { "ServiceRegistrations", serviceRegistrations }
        };

        await GenerateGlobalUsingsAsync(plan, blazorProjectPath, replacements);
        await GenerateImportsRazorAsync(blazorProjectPath, replacements);
        await GenerateAppRazorAsync(blazorProjectPath, replacements);
        await GenerateRoutesRazorAsync(blazorProjectPath, replacements);
        await GenerateProgramCsAsync(blazorProjectPath, replacements);
    }

    private async Task GenerateGlobalUsingsAsync(
        GenerationPlan plan,
        string blazorProjectPath,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(blazorProjectPath, "GlobalUsings.cs");
        string content = _templateEngine.Render(_templateLoader.Load("Foundation/GlobalUsings"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateImportsRazorAsync(
        string blazorProjectPath,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(blazorProjectPath, "_Imports.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Foundation/ImportsRazor"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppRazorAsync(
        string blazorProjectPath,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "App.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Foundation/AppRazor"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateRoutesRazorAsync(
        string blazorProjectPath,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Routes.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Foundation/RoutesRazor"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateProgramCsAsync(
        string blazorProjectPath,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(blazorProjectPath, "Program.cs");
        string content = _templateEngine.Render(_templateLoader.Load("Foundation/ProgramCs"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
