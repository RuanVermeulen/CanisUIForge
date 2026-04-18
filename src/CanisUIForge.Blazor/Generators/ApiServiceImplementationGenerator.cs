using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class ApiServiceImplementationGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public ApiServiceImplementationGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, ResolvedResource resource, string blazorProjectPath)
    {
        string filePath = Path.Combine(blazorProjectPath, "Services", $"{resource.Name}ApiService.cs");

        string implementationMethods = ApiServiceGenerationHelper.BuildImplementationMethods(resource);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "ResourceName", resource.Name },
            { "ImplementationMethods", implementationMethods }
        };

        string template = _templateLoader.Load("Services/ApiServiceImplementation");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
