namespace CanisUIForge.Maui.Generators;

public class MauiEmptyStateViewGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiEmptyStateViewGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string componentsDir = Path.Combine(mauiProjectPath, "Components");
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string xamlPath = Path.Combine(componentsDir, "EmptyStateView.xaml");
        string xamlTemplate = _templateLoader.Load("Components/EmptyStateView");
        string xamlContent = _templateEngine.Render(xamlTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(xamlPath, xamlContent);

        string csPath = Path.Combine(componentsDir, "EmptyStateView.xaml.cs");
        string csTemplate = _templateLoader.Load("Components/EmptyStateViewCs");
        string csContent = _templateEngine.Render(csTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(csPath, csContent);
    }
}
