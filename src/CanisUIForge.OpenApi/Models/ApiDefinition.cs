namespace CanisUIForge.OpenApi.Models;

public class ApiDefinition
{
    public string Title { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ResourceDefinition> Resources { get; set; } = new List<ResourceDefinition>();
}
