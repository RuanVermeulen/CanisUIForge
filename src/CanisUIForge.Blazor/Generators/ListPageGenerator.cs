namespace CanisUIForge.Blazor.Generators;

public class ListPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public ListPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Pages", $"{resource.Name}List.razor");

        ResolvedEndpoint? listEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.List);
        ResolvedEndpoint? deleteEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Delete);
        string responseTypeName = PageGenerationHelper.GetResponseTypeName(listEndpoint, resource.Name);
        string idPropertyName = PageGenerationHelper.GetIdPropertyName(listEndpoint?.ResponseType);
        string gridColumnInitializers = PageGenerationHelper.BuildGridColumnInitializers(responseTypeName, listEndpoint?.ResponseType);

        string listMethodName = listEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(listEndpoint, resource.Name)
            : $"GetAll{resource.Name}sAsync";
        string deleteMethodName = deleteEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(deleteEndpoint, resource.Name)
            : $"Delete{resource.Name}Async";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResponseTypeName", responseTypeName },
            { "GridColumnInitializers", gridColumnInitializers },
            { "IdAccessor", $"item.{idPropertyName}" },
            { "ListMethodName", listMethodName },
            { "DeleteMethodName", deleteMethodName }
        };

        string template = _templateLoader.Load("Pages/ListPage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
