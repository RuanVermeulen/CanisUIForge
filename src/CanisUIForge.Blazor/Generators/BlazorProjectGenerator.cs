namespace CanisUIForge.Blazor.Generators;

public class BlazorProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public BlazorProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string projectFilePath = Path.Combine(blazorProjectPath, $"{plan.SolutionName}.Blazor.csproj");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string template = _templateLoader.Load("Foundation/BlazorProject");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(projectFilePath, content);
    }
}
