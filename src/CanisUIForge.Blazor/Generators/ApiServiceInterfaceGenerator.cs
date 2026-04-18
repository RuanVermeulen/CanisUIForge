using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class ApiServiceInterfaceGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public ApiServiceInterfaceGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Services", $"I{resource.Name}ApiService.cs");

        string interfaceMethods = ApiServiceGenerationHelper.BuildInterfaceMethods(resource);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResourceName", resource.Name },
            { "InterfaceMethods", interfaceMethods }
        };

        string template = _templateLoader.Load("Services/ApiServiceInterface");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
