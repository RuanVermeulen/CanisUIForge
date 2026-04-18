using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;
using CanisUIForge.Generation.Templating;

namespace CanisUIForge.Blazor.Generators;

public class StylingGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public StylingGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string cssDir = Path.Combine(blazorProjectPath, "wwwroot", "css");

        await GenerateThemeCssAsync(cssDir);
        await GenerateLayoutCssAsync(cssDir);
        await GenerateComponentsCssAsync(cssDir);
        await GenerateAppCssAsync(cssDir);
    }

    private async Task GenerateThemeCssAsync(string cssDir)
    {
        string filePath = Path.Combine(cssDir, "theme.css");
        string content = _templateLoader.Load("Styling/ThemeCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateLayoutCssAsync(string cssDir)
    {
        string filePath = Path.Combine(cssDir, "layout.css");
        string content = _templateLoader.Load("Styling/LayoutCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateComponentsCssAsync(string cssDir)
    {
        string filePath = Path.Combine(cssDir, "components.css");
        string content = _templateLoader.Load("Styling/ComponentsCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppCssAsync(string cssDir)
    {
        string filePath = Path.Combine(cssDir, "app.css");
        string content = _templateLoader.Load("Styling/AppCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
