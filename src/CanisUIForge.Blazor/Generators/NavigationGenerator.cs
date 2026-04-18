namespace CanisUIForge.Blazor.Generators;

public class NavigationGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public NavigationGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateNavMenuAsync(plan, blazorProjectPath);
        await GenerateNavMenuCssAsync(blazorProjectPath);
    }

    private async Task GenerateNavMenuAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string navMenuFilePath = Path.Combine(layoutDirectory, "NavMenu.razor");

        string navItems = BuildNavigationItems(plan);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "SolutionName", plan.SolutionName },
            { "NavItems", navItems }
        };

        string template = _templateLoader.Load("Foundation/NavMenu");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(navMenuFilePath, content);
    }

    private async Task GenerateNavMenuCssAsync(string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string cssFilePath = Path.Combine(layoutDirectory, "NavMenu.razor.css");

        string content = _templateLoader.Load("Foundation/NavMenuCss");
        await _fileWriter.WriteGeneratedFileAsync(cssFilePath, content);
    }

    private static string BuildNavigationItems(GenerationPlan plan)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ResolvedResource resource in plan.Resources)
        {
            string href = resource.Name.ToLowerInvariant();
            builder.AppendLine($"        <div class=\"nav-item px-3\">");
            builder.AppendLine($"            <NavLink class=\"nav-link\" href=\"{href}\">");
            builder.AppendLine($"                <span class=\"bi bi-list-nested\" aria-hidden=\"true\"></span> {resource.Name}");
            builder.AppendLine($"            </NavLink>");
            builder.AppendLine($"        </div>");
        }

        return builder.ToString().TrimEnd();
    }
}
