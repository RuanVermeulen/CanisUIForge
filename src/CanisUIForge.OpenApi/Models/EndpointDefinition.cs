namespace CanisUIForge.OpenApi.Models;

public class EndpointDefinition
{
    public string Route { get; set; } = string.Empty;

    public HttpMethodType Method { get; set; }

    public string OperationId { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();

    public string RequestSchemaName { get; set; } = string.Empty;

    public string ResponseSchemaName { get; set; } = string.Empty;

    public EndpointClassification Classification { get; set; } = EndpointClassification.Custom;
}
