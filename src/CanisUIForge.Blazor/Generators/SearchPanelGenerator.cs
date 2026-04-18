namespace CanisUIForge.Blazor.Generators;

public class SearchPanelGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public SearchPanelGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateSearchPanelRazorAsync(plan, blazorProjectPath);
        await GenerateSearchPanelCssAsync(blazorProjectPath);
    }

    private async Task GenerateSearchPanelRazorAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "SearchPanel.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string template = _templateLoader.Load("Components/SearchPanel");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateSearchPanelCssAsync(string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "SearchPanel.razor.css");
        string content = _templateLoader.Load("Components/SearchPanelCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
