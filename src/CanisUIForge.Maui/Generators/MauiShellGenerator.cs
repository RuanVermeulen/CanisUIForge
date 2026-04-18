namespace CanisUIForge.Maui.Generators;

public class MauiShellGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiShellGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string shellItems = MauiServiceRegistrationHelper.BuildShellItems(
            plan.Resources, plan.NamespaceRoot);
        string routeRegistrations = MauiServiceRegistrationHelper.BuildRouteRegistrations(
            plan.Resources, plan.NamespaceRoot);

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot },
            { "SolutionName", plan.SolutionName },
            { "ShellItems", shellItems },
            { "RouteRegistrations", routeRegistrations }
        };

        await GenerateAppShellXamlAsync(mauiProjectPath, replacements);
        await GenerateAppShellXamlCsAsync(mauiProjectPath, replacements);
    }

    private async Task GenerateAppShellXamlAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "AppShell.xaml");
        string template = _templateLoader.Load("Foundation/AppShellXaml");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAppShellXamlCsAsync(string mauiProjectPath, Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(mauiProjectPath, "AppShell.xaml.cs");
        string template = _templateLoader.Load("Foundation/AppShellXamlCs");
        string content = _templateEngine.Render(template, replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
