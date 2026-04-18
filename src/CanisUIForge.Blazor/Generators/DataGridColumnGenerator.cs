namespace CanisUIForge.Blazor.Generators;

public class DataGridColumnGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public DataGridColumnGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(string namespaceRoot, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Components", "Shared", "DataGridColumn.cs");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", namespaceRoot }
        };

        string template = _templateLoader.Load("Components/DataGridColumn");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}

