using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class LayoutGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;

    public LayoutGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        await GenerateMainLayoutAsync(plan, blazorProjectPath);
        await GenerateMainLayoutCssAsync(blazorProjectPath);
    }

    private async Task GenerateMainLayoutAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string layoutFilePath = Path.Combine(layoutDirectory, "MainLayout.razor");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "SolutionName", plan.SolutionName }
        };

        string content = _templateEngine.Render(MainLayoutTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(layoutFilePath, content);
    }

    private async Task GenerateMainLayoutCssAsync(string blazorProjectPath)
    {
        string layoutDirectory = Path.Combine(blazorProjectPath, "Layout");
        string cssFilePath = Path.Combine(layoutDirectory, "MainLayout.razor.css");

        string content = GeneratedFileHeader.Css + MainLayoutCssTemplate;
        await _fileWriter.WriteGeneratedFileAsync(cssFilePath, content);
    }

    private const string MainLayoutTemplate =
@"@* AUTO-GENERATED – DO NOT MODIFY *@
@* Any changes to this file will be overwritten during regeneration. *@
@inherits LayoutComponentBase

<div class=""page"">
    <div class=""sidebar"">
        <NavMenu />
    </div>

    <main>
        <div class=""top-row px-4"">
            <h1>{{SolutionName}}</h1>
        </div>

        <article class=""content px-4"">
            @Body
        </article>
    </main>
</div>
";

    private const string MainLayoutCssTemplate =
@"
.page {
    position: relative;
    display: flex;
    flex-direction: column;
}

main {
    flex: 1;
}

.sidebar {
    background-image: linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%);
}

.top-row {
    background-color: #f7f7f7;
    border-bottom: 1px solid #d6d5d5;
    justify-content: flex-end;
    height: 3.5rem;
    display: flex;
    align-items: center;
}

.top-row h1 {
    font-size: 1.2rem;
    margin: 0;
}

article {
    padding: 1rem;
}

@media (min-width: 768px) {
    .page {
        flex-direction: row;
    }

    .sidebar {
        width: 250px;
        height: 100vh;
        position: sticky;
        top: 0;
    }

    .top-row {
        position: sticky;
        top: 0;
        z-index: 1;
    }
}
";
}
