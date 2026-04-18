namespace CanisUIForge.Testing.Generators;

public class ApiServiceTestGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public ApiServiceTestGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string testProjectPath)
    {
        string servicesDirectory = Path.Combine(testProjectPath, "Services");
        _fileWriter.EnsureDirectoryExists(servicesDirectory);

        string filePath = Path.Combine(servicesDirectory, $"{resource.Name}ApiServiceTests.cs");
        string testMethods = TestGenerationHelper.BuildTestMethods(resource);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResourceName", resource.Name },
            { "TestMethods", testMethods }
        };

        string template = _templateLoader.Load("ApiServiceTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
