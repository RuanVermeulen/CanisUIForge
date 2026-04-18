namespace CanisUIForge.Maui.Generators;

public class MauiBootstrapGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiBootstrapGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string serviceRegistrations = MauiServiceRegistrationHelper.BuildServiceRegistrations(
            plan.Resources, plan.NamespaceRoot);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "SolutionName", plan.SolutionName },
            { "ServiceRegistrations", serviceRegistrations }
        };

        await GenerateGlobalUsingsAsync(mauiProjectPath, replacements);
        await GenerateAppXamlAsync(mauiProjectPath, replacements);
        await GenerateAppXamlCsAsync(mauiProjectPath, replacements);
        await GenerateMauiProgramAsync(mauiProjectPath, replacements);
    }

    private async Task GenerateGlobalUsingsAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "GlobalUsings.cs");
        string template = _templateLoader.Load("Foundation/MauiGlobalUsings");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppXamlAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "App.xaml");
        string template = _templateLoader.Load("Foundation/AppXaml");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppXamlCsAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "App.xaml.cs");
        string template = _templateLoader.Load("Foundation/AppXamlCs");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateMauiProgramAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "MauiProgram.cs");
        string template = _templateLoader.Load("Foundation/MauiProgram");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
