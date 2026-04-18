using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class CreatePageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public CreatePageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Pages", $"{resource.Name}Create.razor");

        ResolvedEndpoint? createEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Create);
        string requestTypeName = PageGenerationHelper.GetRequestTypeName(createEndpoint, resource.Name, "Create");
        string formFields = PageGenerationHelper.BuildFormFieldRenderers(createEndpoint?.RequestType);

        string createMethodName = createEndpoint is not null
            ? ApiServiceGenerationHelper.GetMethodName(createEndpoint, resource.Name)
            : $"Create{resource.Name}Async";

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "ResourceName", resource.Name },
            { "ResourceNameLower", resource.Name.ToLowerInvariant() },
            { "NamespaceRoot", plan.NamespaceRoot },
            { "RequestTypeName", requestTypeName },
            { "FormFields", formFields },
            { "CreateMethodName", createMethodName }
        };

        string template = _templateLoader.Load("Pages/CreatePage");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
