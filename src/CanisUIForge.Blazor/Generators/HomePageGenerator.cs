namespace CanisUIForge.Blazor.Generators;

public class HomePageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public HomePageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string pagesDirectory = Path.Combine(blazorProjectPath, "Pages");
        string homePageFilePath = Path.Combine(pagesDirectory, "Home.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "ApiTitle", plan.ApiTitle },
            { "ApiVersion", plan.ApiVersion }
        };

        string template = _templateLoader.Load("Foundation/HomePage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(homePageFilePath, content);
    }
}

