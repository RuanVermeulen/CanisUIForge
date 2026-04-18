namespace CanisUIForge.Generation.Unification;

public class MetadataUnifier : IMetadataUnifier
{
    public List<ResolvedResource> Unify(ApiDefinition apiDefinition, ITypeRegistry typeRegistry)
    {
        if (apiDefinition is null)
        {
            throw new ArgumentNullException(nameof(apiDefinition));
        }

        if (typeRegistry is null)
        {
            throw new ArgumentNullException(nameof(typeRegistry));
        }

        List<ResolvedResource> resolvedResources = new List<ResolvedResource>();

        foreach (ResourceDefinition resource in apiDefinition.Resources)
        {
            ResolvedResource resolvedResource = new ResolvedResource
            {
                Name = resource.Name
            };

            foreach (EndpointDefinition endpoint in resource.Endpoints)
            {
                ResolvedEndpoint resolvedEndpoint = MapEndpoint(endpoint, typeRegistry);
                resolvedResource.Endpoints.Add(resolvedEndpoint);
            }

            resolvedResources.Add(resolvedResource);
        }

        return resolvedResources;
    }

    private static ResolvedEndpoint MapEndpoint(EndpointDefinition endpoint, ITypeRegistry typeRegistry)
    {
        ResolvedEndpoint resolvedEndpoint = new ResolvedEndpoint
        {
            Route = endpoint.Route,
            Method = endpoint.Method,
            OperationId = endpoint.OperationId,
            Summary = endpoint.Summary,
            Parameters = endpoint.Parameters,
            Classification = endpoint.Classification,
            RequestSchemaName = endpoint.RequestSchemaName,
            ResponseSchemaName = endpoint.ResponseSchemaName
        };

        if (!string.IsNullOrWhiteSpace(endpoint.RequestSchemaName))
        {
            resolvedEndpoint.RequestType = typeRegistry.Resolve(endpoint.RequestSchemaName);
        }

        if (!string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName))
        {
            resolvedEndpoint.ResponseType = typeRegistry.Resolve(endpoint.ResponseSchemaName);
        }

        return resolvedEndpoint;
    }
}
