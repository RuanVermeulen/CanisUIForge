namespace CanisUIForge.Maui.Generators;

public class MauiProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string projectFilePath = Path.Combine(mauiProjectPath, $"{plan.SolutionName}.Maui.csproj");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "SolutionNameLower", plan.SolutionName.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string template = _templateLoader.Load("Foundation/MauiProject");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(projectFilePath, content);
    }
}
