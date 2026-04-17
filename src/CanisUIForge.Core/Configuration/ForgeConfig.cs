namespace CanisUIForge.Core.Configuration;

public class ForgeConfig
{
    public string SolutionName { get; set; } = string.Empty;

    public List<TargetPlatform> Targets { get; set; } = new List<TargetPlatform>();

    public string SwaggerSource { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string NamespaceRoot { get; set; } = string.Empty;

    public ContractsConfig Contracts { get; set; } = new ContractsConfig();

    public List<ControllerConfig> Controllers { get; set; } = new List<ControllerConfig>();

    public TestConfig Tests { get; set; } = new TestConfig();
}
