using CanisUIForge.Core.Enums;
using CanisUIForge.OpenApi.Models;

namespace CanisUIForge.Generation.Models;

public class ResolvedEndpoint
{
    public string Route { get; set; } = string.Empty;

    public HttpMethodType Method { get; set; }

    public string OperationId { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();

    public EndpointClassification Classification { get; set; } = EndpointClassification.Custom;

    public string RequestSchemaName { get; set; } = string.Empty;

    public string ResponseSchemaName { get; set; } = string.Empty;

    public Type? RequestType { get; set; }

    public Type? ResponseType { get; set; }
}
