namespace CanisUIForge.Testing.Generators;

public class TestProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public TestProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
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
        await GenerateMockHandlerAsync(testProjectPath, replacements);
        await GenerateTestBaseAsync(testProjectPath, replacements);
    }

    private async Task GenerateProjectFileAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, $"{replacements["SolutionName"]}.Tests.Unit.csproj");
        string template = _templateLoader.Load("TestProject");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateGlobalUsingsAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(testProjectPath, "GlobalUsings.cs");
        string template = _templateLoader.Load("TestGlobalUsings");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateMockHandlerAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string helpersDirectory = Path.Combine(testProjectPath, "Helpers");
        _fileWriter.EnsureDirectoryExists(helpersDirectory);

        string filePath = Path.Combine(helpersDirectory, "MockHttpMessageHandler.cs");
        string template = _templateLoader.Load("MockHttpMessageHandler");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateTestBaseAsync(string testProjectPath, Dictionary<string, string> replacements)
    {
        string helpersDirectory = Path.Combine(testProjectPath, "Helpers");
        _fileWriter.EnsureDirectoryExists(helpersDirectory);

        string filePath = Path.Combine(helpersDirectory, "ApiServiceTestBase.cs");
        string template = _templateLoader.Load("ApiServiceTestBase");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
