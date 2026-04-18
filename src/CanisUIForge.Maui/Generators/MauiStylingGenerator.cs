namespace CanisUIForge.Maui.Generators;

public class MauiStylingGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiStylingGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string stylesDir = Path.Combine(mauiProjectPath, "Resources", "Styles");

        string colorsPath = Path.Combine(stylesDir, "Colors.xaml");
        string colorsContent = _templateLoader.Load("Styling/Colors");
        await _fileWriter.WriteGeneratedFileAsync(colorsPath, colorsContent);

        string stylesPath = Path.Combine(stylesDir, "Styles.xaml");
        string stylesContent = _templateLoader.Load("Styling/Styles");
        await _fileWriter.WriteGeneratedFileAsync(stylesPath, stylesContent);
    }
}
