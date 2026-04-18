namespace CanisUIForge.Cli.Parsing;

public class CliOptions
{
    public CliCommand Command { get; set; }

    public string ConfigFilePath { get; set; } = string.Empty;

    public string SolutionName { get; set; } = string.Empty;

    public string SwaggerSource { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string NamespaceRoot { get; set; } = string.Empty;

    public string ContractsMode { get; set; } = string.Empty;

    public string ContractsProjectPath { get; set; } = string.Empty;

    public string ContractsPackageId { get; set; } = string.Empty;

    public string ContractsPackageVersion { get; set; } = string.Empty;

    public string ContractsLocalFeed { get; set; } = string.Empty;

    public string Targets { get; set; } = string.Empty;

    public bool UnitTests { get; set; }

    public bool PlaywrightTests { get; set; }

    public bool AppiumTests { get; set; }
}
