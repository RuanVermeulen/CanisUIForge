using CanisUIForge.Core.Enums;

namespace CanisUIForge.Generation.Models;

public class ResolvedResource
{
    public string Name { get; set; } = string.Empty;

    public GenerationStyle Style { get; set; } = GenerationStyle.FormAndGrid;

    public List<ResolvedEndpoint> Endpoints { get; set; } = new List<ResolvedEndpoint>();
}
