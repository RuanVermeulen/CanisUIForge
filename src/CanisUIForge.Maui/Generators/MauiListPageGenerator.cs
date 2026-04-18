namespace CanisUIForge.Maui.Generators;

public class MauiListPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiListPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string mauiProjectPath)
    {
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");

        ResolvedEndpoint? listEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.List);
        ResolvedEndpoint? deleteEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Delete);
        string responseTypeName = MauiPageGenerationHelper.GetResponseTypeName(listEndpoint, resource.Name);
        string idPropertyName = MauiPageGenerationHelper.GetIdPropertyName(listEndpoint?.ResponseType);
        string itemTemplateContent = MauiPageGenerationHelper.BuildItemTemplateContent(listEndpoint?.ResponseType);

        string listMethodName = listEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(listEndpoint, resource.Name)
            : $"GetAll{resource.Name}sAsync";
        string deleteMethodName = deleteEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(deleteEndpoint, resource.Name)
            : $"Delete{resource.Name}Async";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResponseTypeName", responseTypeName },
            { "ItemTemplateContent", itemTemplateContent },
            { "IdPropertyName", idPropertyName },
            { "ListMethodName", listMethodName },
            { "DeleteMethodName", deleteMethodName }
        };

        string xamlPath = Path.Combine(pagesDirectory, $"{resource.Name}ListPage.xaml");
        string xamlTemplate = _templateLoader.Load("Pages/ListPage");
        string xamlContent = _templateEngine.Render(xamlTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(xamlPath, xamlContent);

        string csPath = Path.Combine(pagesDirectory, $"{resource.Name}ListPage.xaml.cs");
        string csTemplate = _templateLoader.Load("Pages/ListPageCs");
        string csContent = _templateEngine.Render(csTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(csPath, csContent);
    }
}
