namespace CanisUIForge.OpenApi.Classification;

public class EndpointClassifier : IEndpointClassifier
{
    public EndpointClassification Classify(HttpMethodType method, string route, string operationId)
    {
        string normalizedRoute = (route ?? string.Empty).ToLowerInvariant().TrimEnd('/');
        string normalizedOperationId = (operationId ?? string.Empty).ToLowerInvariant();

        return method switch
        {
            HttpMethodType.Get => ClassifyGetEndpoint(normalizedRoute, normalizedOperationId),
            HttpMethodType.Post => ClassifyPostEndpoint(normalizedRoute, normalizedOperationId),
            HttpMethodType.Put => EndpointClassification.Update,
            HttpMethodType.Patch => EndpointClassification.Update,
            HttpMethodType.Delete => EndpointClassification.Delete,
            _ => EndpointClassification.Custom
        };
    }

    private static EndpointClassification ClassifyGetEndpoint(string route, string operationId)
    {
        if (ContainsSearchIndicator(route, operationId))
        {
            return EndpointClassification.Search;
        }

        if (EndsWithIdParameter(route))
        {
            return EndpointClassification.GetById;
        }

        return EndpointClassification.List;
    }

    private static EndpointClassification ClassifyPostEndpoint(string route, string operationId)
    {
        if (ContainsSearchIndicator(route, operationId))
        {
            return EndpointClassification.Search;
        }

        return EndpointClassification.Create;
    }

    private static bool EndsWithIdParameter(string route)
    {
        return route.EndsWith("/{id}", StringComparison.OrdinalIgnoreCase)
            || route.Contains("/{id}", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsSearchIndicator(string route, string operationId)
    {
        return route.Contains("/search", StringComparison.OrdinalIgnoreCase)
            || route.Contains("/filter", StringComparison.OrdinalIgnoreCase)
            || route.Contains("/query", StringComparison.OrdinalIgnoreCase)
            || operationId.Contains("search", StringComparison.OrdinalIgnoreCase)
            || operationId.Contains("filter", StringComparison.OrdinalIgnoreCase);
    }
}
