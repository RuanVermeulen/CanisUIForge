using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class LayoutGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public LayoutGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateMainLayoutAsync(plan, blazorProjectPath);
        await GenerateMainLayoutCssAsync(blazorProjectPath);
    }

    private async Task GenerateMainLayoutAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string layoutFilePath = Path.Combine(layoutDirectory, "MainLayout.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "SolutionName", plan.SolutionName }
        };

        string template = _templateLoader.Load("Foundation/MainLayout");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(layoutFilePath, content);
    }

    private async Task GenerateMainLayoutCssAsync(string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string cssFilePath = Path.Combine(layoutDirectory, "MainLayout.razor.css");

        string content = _templateLoader.Load("Foundation/MainLayoutCss");
        await _fileWriter.WriteGeneratedFileAsync(cssFilePath, content);
    }
}
