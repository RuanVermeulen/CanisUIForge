namespace CanisUIForge.Testing.Generators;

public class UnitTestGenerator : IUnitTestGenerator
{
    private readonly TestProjectGenerator _testProjectGenerator;
    private readonly ApiServiceTestGenerator _apiServiceTestGenerator;
    private readonly IFileWriter _fileWriter;

    public UnitTestGenerator(
        TestProjectGenerator testProjectGenerator,
        ApiServiceTestGenerator apiServiceTestGenerator,
        IFileWriter fileWriter)
    {
        _testProjectGenerator = testProjectGenerator ?? throw new ArgumentNullException(nameof(testProjectGenerator));
        _apiServiceTestGenerator = apiServiceTestGenerator ?? throw new ArgumentNullException(nameof(apiServiceTestGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string testProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Tests.Unit");
        _fileWriter.EnsureDirectoryExists(testProjectPath);

        await _testProjectGenerator.GenerateAsync(plan, testProjectPath);

        foreach (ResolvedResource resource in plan.Resources)
        {
            await _apiServiceTestGenerator.GenerateAsync(plan, resource, testProjectPath);
        }
    }
}
