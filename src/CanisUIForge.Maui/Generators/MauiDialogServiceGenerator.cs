namespace CanisUIForge.Maui.Generators;

public class MauiDialogServiceGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public MauiDialogServiceGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string mauiProjectPath)
    {
        string servicesDir = Path.Combine(mauiProjectPath, "Services");
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        string interfacePath = Path.Combine(servicesDir, "IDialogService.cs");
        string interfaceTemplate = _templateLoader.Load("Services/IDialogService");
        string interfaceContent = _templateEngine.Render(interfaceTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(interfacePath, interfaceContent);

        string implPath = Path.Combine(servicesDir, "MauiDialogService.cs");
        string implTemplate = _templateLoader.Load("Services/MauiDialogService");
        string implContent = _templateEngine.Render(implTemplate, replacements);
        await _fileWriter.WriteGeneratedFileAsync(implPath, implContent);
    }
}
