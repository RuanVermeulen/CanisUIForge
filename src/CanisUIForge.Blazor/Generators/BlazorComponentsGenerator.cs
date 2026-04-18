namespace CanisUIForge.Blazor.Generators;

public class BlazorComponentsGenerator : IBlazorComponentsGenerator
{
    private readonly DataGridGenerator _dataGridGenerator;
    private readonly DataGridColumnGenerator _dataGridColumnGenerator;
    private readonly FormGenerator _formGenerator;
    private readonly FieldRendererGenerator _fieldRendererGenerator;
    private readonly SearchPanelGenerator _searchPanelGenerator;
    private readonly LoadingPanelGenerator _loadingPanelGenerator;
    private readonly ErrorPanelGenerator _errorPanelGenerator;
    private readonly EmptyStateGenerator _emptyStateGenerator;
    private readonly DialogSystemGenerator _dialogSystemGenerator;
    private readonly StylingGenerator _stylingGenerator;
    private readonly IFileWriter _fileWriter;

    public BlazorComponentsGenerator(
        DataGridGenerator dataGridGenerator,
        DataGridColumnGenerator dataGridColumnGenerator,
        FormGenerator formGenerator,
        FieldRendererGenerator fieldRendererGenerator,
        SearchPanelGenerator searchPanelGenerator,
        LoadingPanelGenerator loadingPanelGenerator,
        ErrorPanelGenerator errorPanelGenerator,
        EmptyStateGenerator emptyStateGenerator,
        DialogSystemGenerator dialogSystemGenerator,
        StylingGenerator stylingGenerator,
        IFileWriter fileWriter)
    {
        _dataGridGenerator = dataGridGenerator ?? throw new ArgumentNullException(nameof(dataGridGenerator));
        _dataGridColumnGenerator = dataGridColumnGenerator ?? throw new ArgumentNullException(nameof(dataGridColumnGenerator));
        _formGenerator = formGenerator ?? throw new ArgumentNullException(nameof(formGenerator));
        _fieldRendererGenerator = fieldRendererGenerator ?? throw new ArgumentNullException(nameof(fieldRendererGenerator));
        _searchPanelGenerator = searchPanelGenerator ?? throw new ArgumentNullException(nameof(searchPanelGenerator));
        _loadingPanelGenerator = loadingPanelGenerator ?? throw new ArgumentNullException(nameof(loadingPanelGenerator));
        _errorPanelGenerator = errorPanelGenerator ?? throw new ArgumentNullException(nameof(errorPanelGenerator));
        _emptyStateGenerator = emptyStateGenerator ?? throw new ArgumentNullException(nameof(emptyStateGenerator));
        _dialogSystemGenerator = dialogSystemGenerator ?? throw new ArgumentNullException(nameof(dialogSystemGenerator));
        _stylingGenerator = stylingGenerator ?? throw new ArgumentNullException(nameof(stylingGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string blazorProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Blazor");

        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "Components", "Shared"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "Services"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(blazorProjectPath, "wwwroot", "css"));

        await _dataGridGenerator.GenerateAsync(plan, blazorProjectPath);
        await _dataGridColumnGenerator.GenerateAsync(plan.NamespaceRoot, blazorProjectPath);
        await _formGenerator.GenerateAsync(plan, blazorProjectPath);
        await _fieldRendererGenerator.GenerateAsync(plan, blazorProjectPath);
        await _searchPanelGenerator.GenerateAsync(plan, blazorProjectPath);
        await _loadingPanelGenerator.GenerateAsync(plan, blazorProjectPath);
        await _errorPanelGenerator.GenerateAsync(plan, blazorProjectPath);
        await _emptyStateGenerator.GenerateAsync(plan, blazorProjectPath);
        await _dialogSystemGenerator.GenerateAsync(plan, blazorProjectPath);
        await _stylingGenerator.GenerateAsync(plan, blazorProjectPath);
    }
}
