using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class HomePageGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;

    public HomePageGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string pagesDirectory = Path.Combine(blazorProjectPath, "Pages");
        string homePageFilePath = Path.Combine(pagesDirectory, "Home.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "ApiTitle", plan.ApiTitle },
            { "ApiVersion", plan.ApiVersion }
        };

        string content = _templateEngine.Render(HomePageTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(homePageFilePath, content);
    }

    private const string HomePageTemplate =
@"@* AUTO-GENERATED – DO NOT MODIFY *@
@* Any changes to this file will be overwritten during regeneration. *@
@page ""/""

<PageTitle>{{SolutionName}}</PageTitle>

<h1>{{SolutionName}}</h1>

<p>Welcome to <strong>{{SolutionName}}</strong>.</p>

<div class=""home-info"">
    <p>API: <strong>{{ApiTitle}}</strong></p>
    <p>Version: <strong>{{ApiVersion}}</strong></p>
</div>
";
}
