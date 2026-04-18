using CanisUIForge.Generation.Models;
using CanisUIForge.Generation.Output;

namespace CanisUIForge.Blazor.Generators;

public class BlazorPageGenerator : IBlazorPageGenerator
{
    private readonly ListPageGenerator _listPageGenerator;
    private readonly CreatePageGenerator _createPageGenerator;
    private readonly EditPageGenerator _editPageGenerator;
    private readonly SearchPageGenerator _searchPageGenerator;
    private readonly IFileWriter _fileWriter;

    public BlazorPageGenerator(
        ListPageGenerator listPageGenerator,
        CreatePageGenerator createPageGenerator,
        EditPageGenerator editPageGenerator,
        SearchPageGenerator searchPageGenerator,
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
        string blazorProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Blazor");
        string pagesDirectory = Path.Combine(blazorProjectPath, "Pages");
        _fileWriter.EnsureDirectoryExists(pagesDirectory);

        foreach (ResolvedResource resource in plan.Resources)
        {
            bool hasListEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.List) is not null;
            bool hasCreateEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Create) is not null;
            bool hasUpdateEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Update) is not null;
            bool hasSearchEndpoint = PageGenerationHelper.FindEndpoint(resource, EndpointClassification.Search) is not null;

            if (resource.Style == GenerationStyle.Grid
                || resource.Style == GenerationStyle.FormAndGrid
                || hasListEndpoint)
            {
                await _listPageGenerator.GenerateAsync(plan, resource, blazorProjectPath);
            }

            if (resource.Style == GenerationStyle.Form
                || resource.Style == GenerationStyle.FormAndGrid
                || hasCreateEndpoint)
            {
                await _createPageGenerator.GenerateAsync(plan, resource, blazorProjectPath);
            }

            if (resource.Style == GenerationStyle.Form
                || resource.Style == GenerationStyle.FormAndGrid
                || hasUpdateEndpoint)
            {
                await _editPageGenerator.GenerateAsync(plan, resource, blazorProjectPath);
            }

            if (resource.Style == GenerationStyle.Search || hasSearchEndpoint)
            {
                await _searchPageGenerator.GenerateAsync(plan, resource, blazorProjectPath);
            }
        }
    }
}
