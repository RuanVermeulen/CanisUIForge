namespace CanisUIForge.OpenApi.Scanning;

public class OpenApiScanner : IOpenApiScanner
{
    private readonly ISwaggerLoader _swaggerLoader;
    private readonly IEndpointClassifier _endpointClassifier;

    public OpenApiScanner(ISwaggerLoader swaggerLoader, IEndpointClassifier endpointClassifier)
    {
        _swaggerLoader = swaggerLoader ?? throw new ArgumentNullException(nameof(swaggerLoader));
        _endpointClassifier = endpointClassifier ?? throw new ArgumentNullException(nameof(endpointClassifier));
    }

    public async Task<ApiDefinition> ScanAsync(string swaggerSource)
    {
        OpenApiDocument document = await _swaggerLoader.LoadAsync(swaggerSource);

        ApiDefinition apiDefinition = new ApiDefinition
        {
            Title = document.Info?.Title ?? string.Empty,
            Version = document.Info?.Version ?? string.Empty,
            Description = document.Info?.Description ?? string.Empty
        };

        Dictionary<string, ResourceDefinition> resourceMap = new Dictionary<string, ResourceDefinition>(
            StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, OpenApiPathItem> pathEntry in document.Paths)
        {
            string route = pathEntry.Key;
            OpenApiPathItem pathItem = pathEntry.Value;

            foreach (KeyValuePair<OperationType, OpenApiOperation> operationEntry in pathItem.Operations)
            {
                OperationType operationType = operationEntry.Key;
                OpenApiOperation operation = operationEntry.Value;

                string resourceName = ExtractResourceName(operation, route);
                HttpMethodType httpMethod = MapOperationType(operationType);

                EndpointDefinition endpoint = BuildEndpointDefinition(route, httpMethod, operation);

                if (!resourceMap.TryGetValue(resourceName, out ResourceDefinition? resource))
                {
                    resource = new ResourceDefinition { Name = resourceName };
                    resourceMap[resourceName] = resource;
                }

                resource.Endpoints.Add(endpoint);
            }
        }

        apiDefinition.Resources = resourceMap.Values.ToList();

        return apiDefinition;
    }

    private EndpointDefinition BuildEndpointDefinition(
        string route,
        HttpMethodType method,
        OpenApiOperation operation)
    {
        EndpointDefinition endpoint = new EndpointDefinition
        {
            Route = route,
            Method = method,
            OperationId = operation.OperationId ?? string.Empty,
            Summary = operation.Summary ?? string.Empty,
            RequestSchemaName = ExtractRequestSchemaName(operation),
            ResponseSchemaName = ExtractResponseSchemaName(operation),
            Parameters = ExtractParameters(operation)
        };

        endpoint.Classification = _endpointClassifier.Classify(method, route, endpoint.OperationId);

        return endpoint;
    }

    private static string ExtractResourceName(OpenApiOperation operation, string route)
    {
        if (operation.Tags is not null && operation.Tags.Count > 0)
        {
            return operation.Tags[0].Name;
        }

        return ExtractResourceNameFromRoute(route);
    }

    private static string ExtractResourceNameFromRoute(string route)
    {
        string[] segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (string segment in segments)
        {
            if (!segment.StartsWith("{") && !segment.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return segment;
            }
        }

        return "Default";
    }

    private static HttpMethodType MapOperationType(OperationType operationType)
    {
        return operationType switch
        {
            OperationType.Get => HttpMethodType.Get,
            OperationType.Post => HttpMethodType.Post,
            OperationType.Put => HttpMethodType.Put,
            OperationType.Patch => HttpMethodType.Patch,
            OperationType.Delete => HttpMethodType.Delete,
            _ => HttpMethodType.Get
        };
    }

    private static string ExtractRequestSchemaName(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content is null)
        {
            return string.Empty;
        }

        foreach (KeyValuePair<string, OpenApiMediaType> content in operation.RequestBody.Content)
        {
            OpenApiSchema? schema = content.Value.Schema;

            if (schema?.Reference is not null)
            {
                return schema.Reference.Id ?? string.Empty;
            }

            if (schema?.Items?.Reference is not null)
            {
                return schema.Items.Reference.Id ?? string.Empty;
            }
        }

        return string.Empty;
    }

    private static string ExtractResponseSchemaName(OpenApiOperation operation)
    {
        if (operation.Responses is null)
        {
            return string.Empty;
        }

        if (operation.Responses.TryGetValue("200", out OpenApiResponse? response)
            || operation.Responses.TryGetValue("201", out response))
        {
            if (response.Content is not null)
            {
                foreach (KeyValuePair<string, OpenApiMediaType> content in response.Content)
                {
                    OpenApiSchema? schema = content.Value.Schema;

                    if (schema?.Reference is not null)
                    {
                        return schema.Reference.Id ?? string.Empty;
                    }

                    if (schema?.Items?.Reference is not null)
                    {
                        return schema.Items.Reference.Id ?? string.Empty;
                    }
                }
            }
        }

        return string.Empty;
    }

    private static List<ParameterDefinition> ExtractParameters(OpenApiOperation operation)
    {
        List<ParameterDefinition> parameters = new List<ParameterDefinition>();

        if (operation.Parameters is null)
        {
            return parameters;
        }

        foreach (OpenApiParameter parameter in operation.Parameters)
        {
            ParameterDefinition parameterDefinition = new ParameterDefinition
            {
                Name = parameter.Name ?? string.Empty,
                Location = parameter.In?.ToString() ?? string.Empty,
                SchemaType = parameter.Schema?.Type ?? string.Empty,
                IsRequired = parameter.Required
            };

            parameters.Add(parameterDefinition);
        }

        return parameters;
    }
}
