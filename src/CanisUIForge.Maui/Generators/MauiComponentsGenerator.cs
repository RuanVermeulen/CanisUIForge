namespace CanisUIForge.Maui.Generators;

public class MauiComponentsGenerator : IMauiComponentsGenerator
{
    private readonly MauiFormLayoutGenerator _formLayoutGenerator;
    private readonly MauiCollectionListGenerator _collectionListGenerator;
    private readonly MauiSearchBarViewGenerator _searchBarViewGenerator;
    private readonly MauiLoadingIndicatorGenerator _loadingIndicatorGenerator;
    private readonly MauiErrorViewGenerator _errorViewGenerator;
    private readonly MauiEmptyStateViewGenerator _emptyStateViewGenerator;
    private readonly MauiDialogServiceGenerator _dialogServiceGenerator;
    private readonly MauiStylingGenerator _stylingGenerator;
    private readonly IFileWriter _fileWriter;

    public MauiComponentsGenerator(
        MauiFormLayoutGenerator formLayoutGenerator,
        MauiCollectionListGenerator collectionListGenerator,
        MauiSearchBarViewGenerator searchBarViewGenerator,
        MauiLoadingIndicatorGenerator loadingIndicatorGenerator,
        MauiErrorViewGenerator errorViewGenerator,
        MauiEmptyStateViewGenerator emptyStateViewGenerator,
        MauiDialogServiceGenerator dialogServiceGenerator,
        MauiStylingGenerator stylingGenerator,
        IFileWriter fileWriter)
    {
        _formLayoutGenerator = formLayoutGenerator ?? throw new ArgumentNullException(nameof(formLayoutGenerator));
        _collectionListGenerator = collectionListGenerator ?? throw new ArgumentNullException(nameof(collectionListGenerator));
        _searchBarViewGenerator = searchBarViewGenerator ?? throw new ArgumentNullException(nameof(searchBarViewGenerator));
        _loadingIndicatorGenerator = loadingIndicatorGenerator ?? throw new ArgumentNullException(nameof(loadingIndicatorGenerator));
        _errorViewGenerator = errorViewGenerator ?? throw new ArgumentNullException(nameof(errorViewGenerator));
        _emptyStateViewGenerator = emptyStateViewGenerator ?? throw new ArgumentNullException(nameof(emptyStateViewGenerator));
        _dialogServiceGenerator = dialogServiceGenerator ?? throw new ArgumentNullException(nameof(dialogServiceGenerator));
        _stylingGenerator = stylingGenerator ?? throw new ArgumentNullException(nameof(stylingGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string mauiProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Maui");

        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Components"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Services"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Resources", "Styles"));

        await _formLayoutGenerator.GenerateAsync(plan, mauiProjectPath);
        await _collectionListGenerator.GenerateAsync(plan, mauiProjectPath);
        await _searchBarViewGenerator.GenerateAsync(plan, mauiProjectPath);
        await _loadingIndicatorGenerator.GenerateAsync(plan, mauiProjectPath);
        await _errorViewGenerator.GenerateAsync(plan, mauiProjectPath);
        await _emptyStateViewGenerator.GenerateAsync(plan, mauiProjectPath);
        await _dialogServiceGenerator.GenerateAsync(plan, mauiProjectPath);
        await _stylingGenerator.GenerateAsync(plan, mauiProjectPath);
    }
}
