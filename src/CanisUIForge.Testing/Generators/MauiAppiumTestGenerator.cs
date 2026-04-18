namespace CanisUIForge.Testing.Generators;

public class MauiAppiumTestGenerator : IAppiumTestGenerator
{
    private readonly AppiumProjectGenerator _projectGenerator;
    private readonly AppiumTestGenerator _testGenerator;
    private readonly IFileWriter _fileWriter;

    public MauiAppiumTestGenerator(
        AppiumProjectGenerator projectGenerator,
        AppiumTestGenerator testGenerator,
        IFileWriter fileWriter)
    {
        _projectGenerator = projectGenerator ?? throw new ArgumentNullException(nameof(projectGenerator));
        _testGenerator = testGenerator ?? throw new ArgumentNullException(nameof(testGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string testProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Tests.Appium");
        _fileWriter.EnsureDirectoryExists(testProjectPath);

        await _projectGenerator.GenerateAsync(plan, testProjectPath);
        await _testGenerator.GenerateNavigationTestAsync(plan, testProjectPath);

        foreach (ResolvedResource resource in plan.Resources)
        {
            if (AppiumTestGenerationHelper.HasListEndpoint(resource)
                || AppiumTestGenerationHelper.HasCrudEndpoints(resource))
            {
                await _testGenerator.GenerateCrudTestAsync(plan, resource, testProjectPath);
            }
        }
    }
}
