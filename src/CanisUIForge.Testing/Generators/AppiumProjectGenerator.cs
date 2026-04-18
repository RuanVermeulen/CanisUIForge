namespace CanisUIForge.Testing.Generators;

public class AppiumProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public AppiumProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
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
            { "SolutionNameLower", plan.SolutionName.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        await GenerateProjectFileAsync(testProjectPath, replacements);
        await GenerateGlobalUsingsAsync(testProjectPath, replacements);
        await GenerateFixtureAsync(testProjectPath, replacements);
        await GenerateHelperAsync(testProjectPath, replacements);
        await GenerateAppLaunchTestAsync(testProjectPath, replacements);
    }

    private async Task GenerateProjectFileAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, $"{replacements["SolutionName"]}.Tests.Appium.csproj");
        string template = _templateLoader.Load("Appium/AppiumProject");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateGlobalUsingsAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, "GlobalUsings.cs");
        string template = _templateLoader.Load("Appium/AppiumGlobalUsings");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateFixtureAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string infrastructureDirectory = Path.Combine(testProjectPath, "Infrastructure");
        _fileWriter.EnsureDirectoryExists(infrastructureDirectory);

        string filePath = Path.Combine(infrastructureDirectory, "AppiumFixture.cs");
        string template = _templateLoader.Load("Appium/AppiumFixture");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateHelperAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string infrastructureDirectory = Path.Combine(testProjectPath, "Infrastructure");
        _fileWriter.EnsureDirectoryExists(infrastructureDirectory);

        string filePath = Path.Combine(infrastructureDirectory, "AppiumTestHelper.cs");
        string template = _templateLoader.Load("Appium/AppiumTestHelper");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppLaunchTestAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, "AppLaunchTests.cs");
        string template = _templateLoader.Load("Appium/AppLaunchTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
