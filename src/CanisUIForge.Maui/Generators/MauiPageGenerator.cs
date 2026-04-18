namespace CanisUIForge.Maui.Generators;

public class MauiPageGenerator : IMauiPageGenerator
{
    private readonly MauiListPageGenerator _listPageGenerator;
    private readonly MauiCreatePageGenerator _createPageGenerator;
    private readonly MauiEditPageGenerator _editPageGenerator;
    private readonly MauiSearchPageGenerator _searchPageGenerator;
    private readonly IFileWriter _fileWriter;

    public MauiPageGenerator(
        MauiListPageGenerator listPageGenerator,
        MauiCreatePageGenerator createPageGenerator,
        MauiEditPageGenerator editPageGenerator,
        MauiSearchPageGenerator searchPageGenerator,
        IFileWriter fileWriter)
    {
        _listPageGenerator = listPageGenerator ?? throw new ArgumentNullException(nameof(listPageGenerator));
        _createPageGenerator = createPageGenerator ?? throw new ArgumentNullException(nameof(createPageGenerator));
        _editPageGenerator = editPageGenerator ?? throw new ArgumentNullException(nameof(editPageGenerator));
        _searchPageGenerator = searchPageGenerator ?? throw new ArgumentNullException(nameof(searchPageGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string mauiProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Maui");
        string pagesDirectory = Path.Combine(mauiProjectPath, "Pages");
        _fileWriter.EnsureDirectoryExists(pagesDirectory);

        foreach (ResolvedResource resource in plan.Resources)
        {
            bool hasListEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.List) is not null;
            bool hasCreateEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Create) is not null;
            bool hasUpdateEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Update) is not null;
            bool hasSearchEndpoint = MauiPageGenerationHelper.FindEndpoint(resource, EndpointClassification.Search) is not null;

            if (resource.Style == GenerationStyle.Grid
                || resource.Style == GenerationStyle.FormAndGrid
                || hasListEndpoint)
            {
                await _listPageGenerator.GenerateAsync(plan, resource, mauiProjectPath);
            }

            if (resource.Style == GenerationStyle.Form
                || resource.Style == GenerationStyle.FormAndGrid
                || hasCreateEndpoint)
            {
                await _createPageGenerator.GenerateAsync(plan, resource, mauiProjectPath);
            }

            if (resource.Style == GenerationStyle.Form
                || resource.Style == GenerationStyle.FormAndGrid
                || hasUpdateEndpoint)
            {
                await _editPageGenerator.GenerateAsync(plan, resource, mauiProjectPath);
            }

            if (resource.Style == GenerationStyle.Search || hasSearchEndpoint)
            {
                await _searchPageGenerator.GenerateAsync(plan, resource, mauiProjectPath);
            }
        }
    }
}
