namespace CanisUIForge.Testing.Generators;

public class PlaywrightProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public PlaywrightProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string testProjectPath)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        await GenerateProjectFileAsync(testProjectPath, replacements);
        await GenerateGlobalUsingsAsync(testProjectPath, replacements);
        await GenerateFixtureAsync(testProjectPath, replacements);
        await GenerateHelperAsync(testProjectPath, replacements);
        await GenerateAppLoadTestAsync(testProjectPath, replacements);
    }

    private async Task GenerateProjectFileAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, $"{replacements["SolutionName"]}.Tests.Playwright.csproj");
        string template = _templateLoader.Load("Playwright/PlaywrightProject");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateGlobalUsingsAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, "GlobalUsings.cs");
        string template = _templateLoader.Load("Playwright/PlaywrightGlobalUsings");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateFixtureAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string infrastructureDirectory = Path.Combine(testProjectPath, "Infrastructure");
        _fileWriter.EnsureDirectoryExists(infrastructureDirectory);

        string filePath = Path.Combine(infrastructureDirectory, "PlaywrightFixture.cs");
        string template = _templateLoader.Load("Playwright/PlaywrightFixture");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateHelperAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string infrastructureDirectory = Path.Combine(testProjectPath, "Infrastructure");
        _fileWriter.EnsureDirectoryExists(infrastructureDirectory);

        string filePath = Path.Combine(infrastructureDirectory, "PlaywrightTestHelper.cs");
        string template = _templateLoader.Load("Playwright/PlaywrightTestHelper");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppLoadTestAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, "AppLoadTests.cs");
        string template = _templateLoader.Load("Playwright/AppLoadTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
