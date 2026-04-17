using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class BlazorProjectGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;

    public BlazorProjectGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string projectFilePath = Path.Combine(blazorProjectPath, $"{plan.SolutionName}.Blazor.csproj");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string content = _templateEngine.Render(ProjectTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(projectFilePath, content);
    }

    private const string ProjectTemplate =
@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>{{NamespaceRoot}}.Blazor</RootNamespace>
    <ImplicitUsings>disable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
";
}
