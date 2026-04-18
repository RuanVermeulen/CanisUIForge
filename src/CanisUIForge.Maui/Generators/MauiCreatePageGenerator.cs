namespace CanisUIForge.Maui.Generators;

public class MauiCreatePageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiCreatePageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string mauiProjectPath)
    {
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");

        ResolvedEndpoint? createEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Create);
        string requestTypeName = MauiPageGenerationHelper.GetRequestTypeName(createEndpoint, resource.Name, "Create");
        string formFields = MauiPageGenerationHelper.BuildFormFields(createEndpoint?.RequestType);
        string formFieldAssignments = MauiPageGenerationHelper.BuildFormFieldAssignments(createEndpoint?.RequestType);

        string createMethodName = createEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(createEndpoint, resource.Name)
            : $"Create{resource.Name}Async";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "RequestTypeName", requestTypeName },
            { "FormFields", formFields },
            { "FormFieldAssignments", formFieldAssignments },
            { "CreateMethodName", createMethodName }
        };

        string xamlPath = Path.Combine(pagesDirectory, $"{resource.Name}CreatePage.xaml");
        string xamlTemplate = _templateLoader.Load("Pages/CreatePage");
        string xamlContent = _templateEngine.Render(xamlTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(xamlPath, xamlContent);

        string csPath = Path.Combine(pagesDirectory, $"{resource.Name}CreatePage.xaml.cs");
        string csTemplate = _templateLoader.Load("Pages/CreatePageCs");
        string csContent = _templateEngine.Render(csTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(csPath, csContent);
    }
}
