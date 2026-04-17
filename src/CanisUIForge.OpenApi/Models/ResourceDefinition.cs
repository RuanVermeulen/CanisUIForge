namespace CanisUIForge.OpenApi.Models;

public class ResourceDefinition
{
    public string Name { get; set; } = string.Empty;

    public List<EndpointDefinition> Endpoints { get; set; } = new List<EndpointDefinition>();
}
