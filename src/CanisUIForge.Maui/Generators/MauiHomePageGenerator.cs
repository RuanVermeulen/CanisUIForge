namespace CanisUIForge.Maui.Generators;

public class MauiHomePageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiHomePageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");
        _fileWriter.EnsureDirectoryExists(pagesDirectory);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ApiTitle", plan.ApiTitle },
            { "ApiVersion", plan.ApiVersion }
        };

        await GenerateHomePageXamlAsync(pagesDirectory, replacements);
        await GenerateHomePageCsAsync(pagesDirectory, replacements);
    }

    private async Task GenerateHomePageXamlAsync(string pagesDirectory, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(pagesDirectory, "HomePage.xaml");
        string template = _templateLoader.Load("Foundation/HomePage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateHomePageCsAsync(string pagesDirectory, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(pagesDirectory, "HomePage.xaml.cs");
        string template = _templateLoader.Load("Foundation/HomePageCs");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
