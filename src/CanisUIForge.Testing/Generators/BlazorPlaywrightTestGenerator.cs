namespace CanisUIForge.Testing.Generators;

public class BlazorPlaywrightTestGenerator : IPlaywrightTestGenerator
{
    private readonly PlaywrightProjectGenerator _projectGenerator;
    private readonly PlaywrightTestGenerator _testGenerator;
    private readonly IFileWriter _fileWriter;

    public BlazorPlaywrightTestGenerator(
        PlaywrightProjectGenerator projectGenerator,
        PlaywrightTestGenerator testGenerator,
        IFileWriter fileWriter)
    {
        _projectGenerator = projectGenerator ?? throw new ArgumentNullException(nameof(projectGenerator));
        _testGenerator = testGenerator ?? throw new ArgumentNullException(nameof(testGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string testProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Tests.Playwright");
        _fileWriter.EnsureDirectoryExists(testProjectPath);

        await _projectGenerator.GenerateAsync(plan, testProjectPath);

        foreach (ResolvedResource resource in plan.Resources)
        {
            await _testGenerator.GenerateAsync(plan, resource, testProjectPath);
        }
    }
}
