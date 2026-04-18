namespace CanisUIForge.Maui.Generators;

public class MauiSearchPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiSearchPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string mauiProjectPath)
    {
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");

        ResolvedEndpoint? searchEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Search);
        ResolvedEndpoint? listEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.List);
        ResolvedEndpoint? responseEndpoint = searchEndpoint ?? listEndpoint;

        string responseTypeName = MauiPageGenerationHelper.GetResponseTypeName(responseEndpoint, resource.Name);
        string idPropertyName = MauiPageGenerationHelper.GetIdPropertyName(responseEndpoint?.ResponseType);
        string itemTemplateContent = MauiPageGenerationHelper.BuildItemTemplateContent(responseEndpoint?.ResponseType);

        string searchMethodName = searchEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(searchEndpoint, resource.Name)
            : $"Search{resource.Name}sAsync";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResponseTypeName", responseTypeName },
            { "ItemTemplateContent", itemTemplateContent },
            { "IdPropertyName", idPropertyName },
            { "SearchMethodName", searchMethodName }
        };

        string xamlPath = Path.Combine(pagesDirectory, $"{resource.Name}SearchPage.xaml");
        string xamlTemplate = _templateLoader.Load("Pages/SearchPage");
        string xamlContent = _templateEngine.Render(xamlTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(xamlPath, xamlContent);

        string csPath = Path.Combine(pagesDirectory, $"{resource.Name}SearchPage.xaml.cs");
        string csTemplate = _templateLoader.Load("Pages/SearchPageCs");
        string csContent = _templateEngine.Render(csTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(csPath, csContent);
    }
}
