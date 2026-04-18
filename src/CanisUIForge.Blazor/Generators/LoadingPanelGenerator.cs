namespace CanisUIForge.Blazor.Generators;

public class LoadingPanelGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public LoadingPanelGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateLoadingPanelRazorAsync(plan, blazorProjectPath);
        await GenerateLoadingPanelCssAsync(blazorProjectPath);
    }

    private async Task GenerateLoadingPanelRazorAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "LoadingPanel.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string template = _templateLoader.Load("Components/LoadingPanel");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateLoadingPanelCssAsync(string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "LoadingPanel.razor.css");
        string content = _templateLoader.Load("Components/LoadingPanelCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
