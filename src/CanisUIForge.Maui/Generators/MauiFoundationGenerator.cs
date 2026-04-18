namespace CanisUIForge.Maui.Generators;

public class MauiFoundationGenerator : IMauiFoundationGenerator
{
    private readonly MauiProjectGenerator _projectGenerator;
    private readonly MauiBootstrapGenerator _bootstrapGenerator;
    private readonly MauiShellGenerator _shellGenerator;
    private readonly MauiHomePageGenerator _homePageGenerator;
    private readonly IFileWriter _fileWriter;

    public MauiFoundationGenerator(
        MauiProjectGenerator projectGenerator,
        MauiBootstrapGenerator bootstrapGenerator,
        MauiShellGenerator shellGenerator,
        MauiHomePageGenerator homePageGenerator,
        IFileWriter fileWriter)
    {
        _projectGenerator = projectGenerator ?? throw new ArgumentNullException(nameof(projectGenerator));
        _bootstrapGenerator = bootstrapGenerator ?? throw new ArgumentNullException(nameof(bootstrapGenerator));
        _shellGenerator = shellGenerator ?? throw new ArgumentNullException(nameof(shellGenerator));
        _homePageGenerator = homePageGenerator ?? throw new ArgumentNullException(nameof(homePageGenerator));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
    }

    public async Task GenerateAsync(GenerationPlan plan)
    {
        string mauiProjectPath = Path.Combine(plan.OutputPath, $"{plan.SolutionName}.Maui");

        _fileWriter.EnsureDirectoryExists(mauiProjectPath);
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Pages"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Services"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Components"));
        _fileWriter.EnsureDirectoryExists(Path.Combine(mauiProjectPath, "Resources", "Styles"));

        await _projectGenerator.GenerateAsync(plan, mauiProjectPath);
        await _bootstrapGenerator.GenerateAsync(plan, mauiProjectPath);
        await _shellGenerator.GenerateAsync(plan, mauiProjectPath);
        await _homePageGenerator.GenerateAsync(plan, mauiProjectPath);
    }
}
