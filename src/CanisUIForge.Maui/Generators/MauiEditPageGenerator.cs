namespace CanisUIForge.Maui.Generators;

public class MauiEditPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiEditPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string mauiProjectPath)
    {
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");

        ResolvedEndpoint? updateEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Update);
        ResolvedEndpoint? getByIdEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.GetById);

        string requestTypeName = MauiPageGenerationHelper.GetRequestTypeName(updateEndpoint, resource.Name, "Update");
        string responseTypeName = MauiPageGenerationHelper.GetResponseTypeName(getByIdEndpoint, resource.Name);
        string formFields = MauiPageGenerationHelper.BuildFormFields(updateEndpoint?.RequestType);
        string formFieldAssignments = MauiPageGenerationHelper.BuildFormFieldAssignments(updateEndpoint?.RequestType);
        string formFieldPopulation = MauiPageGenerationHelper.BuildFormFieldPopulation(updateEndpoint?.RequestType);
        string idPropertyType = MauiPageGenerationHelper.GetIdPropertyTypeName(getByIdEndpoint?.ResponseType);
        string idParseExpression = MauiPageGenerationHelper.GetIdParseExpression(getByIdEndpoint?.ResponseType);

        string getByIdMethodName = getByIdEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(getByIdEndpoint, resource.Name)
            : $"Get{resource.Name}ByIdAsync";
        string updateMethodName = updateEndpoint is not null
            ? MauiPageGenerationHelper.GetMethodName(updateEndpoint, resource.Name)
            : $"Update{resource.Name}Async";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "RequestTypeName", requestTypeName },
            { "ResponseTypeName", responseTypeName },
            { "FormFields", formFields },
            { "FormFieldAssignments", formFieldAssignments },
            { "FormFieldPopulation", formFieldPopulation },
            { "IdPropertyType", idPropertyType },
            { "IdParseExpression", idParseExpression },
            { "GetByIdMethodName", getByIdMethodName },
            { "UpdateMethodName", updateMethodName }
        };

        string xamlPath = Path.Combine(pagesDirectory, $"{resource.Name}EditPage.xaml");
        string xamlTemplate = _templateLoader.Load("Pages/EditPage");
        string xamlContent = _templateEngine.Render(xamlTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(xamlPath, xamlContent);

        string csPath = Path.Combine(pagesDirectory, $"{resource.Name}EditPage.xaml.cs");
        string csTemplate = _templateLoader.Load("Pages/EditPageCs");
        string csContent = _templateEngine.Render(csTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(csPath, csContent);
    }
}
