namespace CanisUIForge.Avalonia.Models;

public class WizardState
{
    public string SolutionName { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string NamespaceRoot { get; set; } = string.Empty;

    public List<TargetPlatform> Targets { get; set; } = new List<TargetPlatform>();

    public string SwaggerSource { get; set; } = string.Empty;

    public ContractsMode ContractsMode { get; set; }

    public string ContractsProjectPath { get; set; } = string.Empty;

    public string ContractsPackageId { get; set; } = string.Empty;

    public string ContractsPackageVersion { get; set; } = string.Empty;

    public string ContractsLocalFeed { get; set; } = string.Empty;

    public List<ControllerSelection> ControllerSelections { get; set; } = new List<ControllerSelection>();

    public bool EnableUnitTests { get; set; }

    public bool EnablePlaywrightTests { get; set; }

    public bool EnableAppiumTests { get; set; }

    public ForgeConfig ToForgeConfig()
    {
        ForgeConfig config = new ForgeConfig
        {
            SolutionName = SolutionName,
            OutputPath = OutputPath,
            NamespaceRoot = !string.IsNullOrWhiteSpace(NamespaceRoot) ? NamespaceRoot : SolutionName,
            SwaggerSource = SwaggerSource,
            Targets = new List<TargetPlatform>(Targets),
            Contracts = new ContractsConfig
            {
                Mode = ContractsMode,
                ProjectPath = ContractsProjectPath,
                PackageId = ContractsPackageId,
                PackageVersion = ContractsPackageVersion,
                LocalFeed = ContractsLocalFeed
            },
            Tests = new TestConfig
            {
                Unit = EnableUnitTests,
                Playwright = EnablePlaywrightTests,
                Appium = EnableAppiumTests
            }
        };

        foreach (ControllerSelection selection in ControllerSelections)
        {
            if (selection.IsSelected)
            {
                config.Controllers.Add(new ControllerConfig
                {
                    Name = selection.Name,
                    Style = selection.Style
                });
            }
        }

        return config;
    }
}
