namespace CanisUIForge.OpenApi.Models;

public class ParameterDefinition
{
    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string SchemaType { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
}
