namespace CanisUIForge.Core.Configuration;

public class ContractsConfig
{
    public ContractsMode Mode { get; set; }

    public string ProjectPath { get; set; } = string.Empty;

    public string PackageId { get; set; } = string.Empty;

    public string PackageVersion { get; set; } = string.Empty;

    public string LocalFeed { get; set; } = string.Empty;
}
