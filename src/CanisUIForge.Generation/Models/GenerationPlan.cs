using CanisUIForge.Core.Configuration;
using CanisUIForge.Core.Enums;

namespace CanisUIForge.Generation.Models;

public class GenerationPlan
{
    public string SolutionName { get; set; } = string.Empty;

    public string NamespaceRoot { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public List<TargetPlatform> Targets { get; set; } = new List<TargetPlatform>();

    public List<ResolvedResource> Resources { get; set; } = new List<ResolvedResource>();

    public TestConfig Tests { get; set; } = new TestConfig();

    public string ApiTitle { get; set; } = string.Empty;

    public string ApiVersion { get; set; } = string.Empty;
}
