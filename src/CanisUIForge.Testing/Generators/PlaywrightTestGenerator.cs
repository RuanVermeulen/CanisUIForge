namespace CanisUIForge.Testing.Generators;

public class PlaywrightTestGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public PlaywrightTestGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string testProjectPath)
    {
        string resourceNameLower = resource.Name.ToLowerInvariant();

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resourceNameLower }
        };

        if (PlaywrightTestGenerationHelper.HasListEndpoint(resource))
        {
            await GenerateListPageTestAsync(testProjectPath, resource, replacements);
        }

        if (PlaywrightTestGenerationHelper.HasCrudEndpoints(resource))
        {
            await GenerateCrudFlowTestAsync(testProjectPath, resource, replacements);
        }
    }

    private async Task GenerateListPageTestAsync(
        string testProjectPath,
        ResolvedResource resource,
        Dictionary<string, string> replacements)
    {
        string pagesDirectory = Path.Combine(testProjectPath, "Pages");
        _fileWriter.EnsureDirectoryExists(pagesDirectory);

        string filePath = Path.Combine(pagesDirectory, $"{resource.Name}ListPageTests.cs");
        string template = _templateLoader.Load("Playwright/ListPageTest");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateCrudFlowTestAsync(
        string testProjectPath,
        ResolvedResource resource,
        Dictionary<string, string> replacements)
    {
        string flowsDirectory = Path.Combine(testProjectPath, "Flows");
        _fileWriter.EnsureDirectoryExists(flowsDirectory);

        string editPageTests = PlaywrightTestGenerationHelper.BuildEditPageTests(resource);
        Dictionary<string, string> crudReplacements = new Dictionary<string, string>(replacements)
        {
            { "EditPageTests", editPageTests }
        };

        string filePath = Path.Combine(flowsDirectory, $"{resource.Name}CrudFlowTests.cs");
        string template = _templateLoader.Load("Playwright/CrudFlowTest");
        string content = _templateEngine.Render(template, crudReplacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
