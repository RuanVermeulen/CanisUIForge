namespace CanisUIForge.Blazor.Generators;

public class SearchPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public SearchPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Pages", $"{resource.Name}Search.razor");

        ResolvedEndpoint? searchEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Search);
        ResolvedEndpoint? listEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.List);
        ResolvedEndpoint? responseEndpoint = searchEndpoint ?? listEndpoint;

        string responseTypeName = PageGenerationHelper.GetResponseTypeName(responseEndpoint, resource.Name);
        string idPropertyName = PageGenerationHelper.GetIdPropertyName(responseEndpoint?.ResponseType);
        string gridColumnInitializers = PageGenerationHelper.BuildGridColumnInitializers(responseTypeName, responseEndpoint?.ResponseType);

        string searchMethodName = searchEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(searchEndpoint, resource.Name)
            : $"Search{resource.Name}sAsync";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResponseTypeName", responseTypeName },
            { "GridColumnInitializers", gridColumnInitializers },
            { "IdAccessor", $"item.{idPropertyName}" },
            { "SearchMethodName", searchMethodName }
        };

        string template = _templateLoader.Load("Pages/SearchPage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
