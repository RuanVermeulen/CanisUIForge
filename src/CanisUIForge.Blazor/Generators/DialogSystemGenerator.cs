namespace CanisUIForge.Blazor.Generators;

public class DialogSystemGenerator
{
    private readonly IFileWriter _fileWriter;
    private readonly ITemplateEngine _templateEngine;
    private readonly ITemplateLoader _templateLoader;

    public DialogSystemGenerator(IFileWriter fileWriter, ITemplateEngine templateEngine, ITemplateLoader templateLoader)
    {
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
    }

    public async Task GenerateAsync(GenerationPlan plan, string blazorProjectPath)
    {
        string servicesDir = Path.Combine(blazorProjectPath, "Services");
        string sharedDir = Path.Combine(blazorProjectPath, "Components", "Shared");

        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            { "NamespaceRoot", plan.NamespaceRoot }
        };

        await GenerateDialogServiceInterfaceAsync(servicesDir, replacements);
        await GenerateDialogServiceAsync(servicesDir, replacements);
        await GenerateDialogHostAsync(sharedDir, replacements);
        await GenerateDialogHostCssAsync(sharedDir);
        await GenerateConfirmDialogAsync(sharedDir, replacements);
        await GenerateAlertDialogAsync(sharedDir, replacements);
    }

    private async Task GenerateDialogServiceInterfaceAsync(
        string servicesDir,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(servicesDir, "IDialogService.cs");
        string content = _templateEngine.Render(_templateLoader.Load("Services/IDialogService"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateDialogServiceAsync(
        string servicesDir,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(servicesDir, "DialogService.cs");
        string content = _templateEngine.Render(_templateLoader.Load("Services/DialogService"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateDialogHostAsync(
        string sharedDir,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(sharedDir, "DialogHost.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Components/DialogHost"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateDialogHostCssAsync(string sharedDir)
    {
        string filePath = Path.Combine(sharedDir, "DialogHost.razor.css");
        string content = _templateLoader.Load("Components/DialogHostCss");
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateConfirmDialogAsync(
        string sharedDir,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(sharedDir, "ConfirmDialog.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Components/ConfirmDialog"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }

    private async Task GenerateAlertDialogAsync(
        string sharedDir,
        Dictionary<string, string> replacements)
    {
        string filePath = Path.Combine(sharedDir, "AlertDialog.razor");
        string content = _templateEngine.Render(_templateLoader.Load("Components/AlertDialog"), replacements);
        await _fileWriter.WriteGeneratedFileAsync(filePath, content);
    }
}
