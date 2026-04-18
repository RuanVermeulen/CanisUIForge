using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class EditPageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public EditPageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Pages", $"{resource.Name}Edit.razor");

        ResolvedEndpoint? updateEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Update);
        ResolvedEndpoint? getByIdEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.GetById);

        string requestTypeName = PageGenerationHelper.GetRequestTypeName(updateEndpoint, resource.Name, "Update");
        string responseTypeName = PageGenerationHelper.GetResponseTypeName(getByIdEndpoint, resource.Name);
        string formFields = PageGenerationHelper.BuildFormFieldRenderers(updateEndpoint?.RequestType);
        string idPropertyType = PageGenerationHelper.GetIdPropertyTypeName(getByIdEndpoint?.ResponseType);
        string idRouteConstraint = PageGenerationHelper.GetIdRouteConstraint(getByIdEndpoint?.ResponseType);

        string getByIdMethodName = getByIdEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(getByIdEndpoint, resource.Name)
            : $"Get{resource.Name}ByIdAsync";
        string updateMethodName = updateEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(updateEndpoint, resource.Name)
            : $"Update{resource.Name}Async";

        string editModelMapping = requestTypeName == responseTypeName
            ? "result"
            : $"new {requestTypeName}()";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "RequestTypeName", requestTypeName },
            { "ResponseTypeName", responseTypeName },
            { "FormFields", formFields },
            { "IdPropertyType", idPropertyType },
            { "IdRouteConstraint", idRouteConstraint },
            { "GetByIdMethodName", getByIdMethodName },
            { "UpdateMethodName", updateMethodName },
            { "EditModelMapping", editModelMapping }
        };

        string template = _templateLoader.Load("Pages/EditPage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
