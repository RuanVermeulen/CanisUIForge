using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;
using System.Text;

namespace CanisUIForge.Blazor.Generators;

public class NavigationGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;

    public NavigationGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
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

        string content = _templateEngine.Render(NavMenuTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(navMenuFilePath, content);
    }

    private async Task GenerateNavMenuCssAsync(string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string cssFilePath = Path.Combine(layoutDirectory, "NavMenu.razor.css");

        string content = GeneratedFileHeader.Css + NavMenuCssTemplate;
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

    private const string NavMenuTemplate =
@"@* AUTO-GENERATED – DO NOT MODIFY *@
@* Any changes to this file will be overwritten during regeneration. *@
<div class=""top-row ps-3 navbar navbar-dark"">
    <div class=""container-fluid"">
        <a class=""navbar-brand"" href="""">{{SolutionName}}</a>
        <button title=""Navigation menu"" class=""navbar-toggler"" @onclick=""ToggleNavMenu"">
            <span class=""navbar-toggler-icon""></span>
        </button>
    </div>
</div>

<div class=""@NavMenuCssClass nav-scrollable"" @onclick=""ToggleNavMenu"">
    <nav class=""flex-column"">
        <div class=""nav-item px-3"">
            <NavLink class=""nav-link"" href="""" Match=""NavLinkMatch.All"">
                <span class=""bi bi-house-door-fill"" aria-hidden=""true""></span> Home
            </NavLink>
        </div>
{{NavItems}}
    </nav>
</div>

@code {
    private bool _collapseNavMenu = true;

    private string? NavMenuCssClass => _collapseNavMenu ? ""collapse"" : null;

    private void ToggleNavMenu()
    {
        _collapseNavMenu = !_collapseNavMenu;
    }
}
";

    private const string NavMenuCssTemplate =
@"
.navbar-toggler {
    appearance: none;
    cursor: pointer;
    width: 3.5rem;
    height: 2.5rem;
    color: white;
    position: absolute;
    top: 0.5rem;
    right: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: url(""data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 30 30'%3e%3cpath stroke='rgba%28255, 255, 255, 0.55%29' stroke-linecap='round' stroke-miterlimit='10' stroke-width='2' d='M4 7h22M4 15h22M4 23h22'/%3e%3c/svg%3e"") no-repeat center/1.75rem rgba(255, 255, 255, 0.1);
}

.navbar-toggler:checked {
    background-color: rgba(255, 255, 255, 0.5);
}

.top-row {
    height: 3.5rem;
    background-color: rgba(0, 0, 0, 0.4);
}

.navbar-brand {
    font-size: 1.1rem;
}

.nav-scrollable {
    display: none;
}

.navbar-toggler:checked ~ .nav-scrollable {
    display: block;
}

@media (min-width: 768px) {
    .navbar-toggler {
        display: none;
    }

    .nav-scrollable {
        display: block;
        height: calc(100vh - 3.5rem);
        overflow-y: auto;
    }
}

.nav-item {
    font-size: 0.9rem;
    padding-bottom: 0.5rem;
}

.nav-item:first-of-type {
    padding-top: 1rem;
}

.nav-item:last-of-type {
    padding-bottom: 1rem;
}

.nav-item ::deep .nav-link {
    color: #d7d7d7;
    border-radius: 4px;
    height: 3rem;
    display: flex;
    align-items: center;
    line-height: 3rem;
}

.nav-item ::deep .active {
    background-color: rgba(255, 255, 255, 0.37);
    color: white;
}
";
}
