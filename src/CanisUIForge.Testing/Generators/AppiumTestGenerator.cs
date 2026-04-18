namespace CanisUIForge.Testing.Generators;

public class AppiumTestGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public AppiumTestGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateNavigationTestAsync(GenerationPlan plan, string testProjectPath)
    {
        string navigationTests = AppiumTestGenerationHelper.BuildNavigationTests(plan.Resources);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "NavigationTests", navigationTests }
        };

        string filePath = Path.Combine(testProjectPath, "NavigationTests.cs");
        string template = _templateLoader.Load("Appium/NavigationTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    public async Task GenerateCrudTestAsync(GenerationPlan plan, ResolvedResource resource, string testProjectPath)
    {
        string resourceNameLower = resource.Name.ToLowerInvariant();
        string crudInteractionTests = AppiumTestGenerationHelper.BuildCrudInteractionTests(resource);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resourceNameLower },
            { "CrudInteractionTests", crudInteractionTests }
        };

        string crudDirectory = Path.Combine(testProjectPath, "Crud");
        _fileWriter.EnsureDirectoryExists(crudDirectory);

        string filePath = Path.Combine(crudDirectory, $"{resource.Name}CrudTests.cs");
        string template = _templateLoader.Load("Appium/CrudTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
