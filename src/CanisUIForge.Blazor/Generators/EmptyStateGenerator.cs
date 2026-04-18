using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class EmptyStateGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public EmptyStateGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateEmptyStateRazorAsync(plan, blazorProjectPath);
        await GenerateEmptyStateCssAsync(blazorProjectPath);
    }

    private async Task GenerateEmptyStateRazorAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "EmptyState.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string template = _templateLoader.Load("Components/EmptyState");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateEmptyStateCssAsync(string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "EmptyState.razor.css");
        string content = _templateLoader.Load("Components/EmptyStateCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
