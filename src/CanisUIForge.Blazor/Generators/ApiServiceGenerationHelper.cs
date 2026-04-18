using CanisUIForge.Generation.Models;

namespace CanisUIForge.Blazor.Generators;

public static class ApiServiceGenerationHelper
{
    public static string BuildInterfaceMethods(ResolvedResource resource)
    {
        StringBuilder builder = new StringBuilder();
        bool isFirst = true;

        foreach (ResolvedEndpoint endpoint in resource.Endpoints)
        {
            if (!isFirst)
            {
                builder.AppendLine();
            }

            isFirst = false;
            string methodSignature = BuildMethodSignature(endpoint, resource.Name);
            builder.AppendLine($"    {methodSignature};");
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildImplementationMethods(ResolvedResource resource)
    {
        StringBuilder builder = new StringBuilder();
        bool isFirst = true;

        foreach (ResolvedEndpoint endpoint in resource.Endpoints)
        {
            if (!isFirst)
            {
                builder.AppendLine();
            }

            isFirst = false;
            string method = BuildMethodImplementation(endpoint, resource.Name);
            builder.Append(method);
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildServiceRegistrations(List<ResolvedResource> resources, string namespaceRoot)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ResolvedResource resource in resources)
        {
            builder.AppendLine($"builder.Services.AddScoped<I{resource.Name}ApiService, {resource.Name}ApiService>();");
        }

        return builder.ToString().TrimEnd();
    }

    private static string BuildMethodSignature(ResolvedEndpoint endpoint, string resourceName)
    {
        string methodName = GetMethodName(endpoint, resourceName);
        string returnType = GetReturnType(endpoint);
        string parameters = BuildParameterList(endpoint);

        return $"{returnType} {methodName}({parameters})";
    }

    private static string BuildMethodImplementation(ResolvedEndpoint endpoint, string resourceName)
    {
        string methodName = GetMethodName(endpoint, resourceName);
        string returnType = GetReturnType(endpoint);
        string parameters = BuildParameterList(endpoint);
        string body = BuildMethodBody(endpoint);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"    public async {returnType} {methodName}({parameters})");
        builder.AppendLine("    {");
        builder.Append(body);
        builder.AppendLine("    }");

        return builder.ToString();
    }

    public static string GetMethodName(ResolvedEndpoint endpoint, string resourceName)
    {
        if (!string.IsNullOrWhiteSpace(endpoint.OperationId))
        {
            string operationId = endpoint.OperationId;
            if (!operationId.EndsWith("Async", StringComparison.Ordinal))
            {
                operationId += "Async";
            }

            return char.ToUpperInvariant(operationId[0]) + operationId.Substring(1);
        }

        return endpoint.Classification switch
        {
            EndpointClassification.List => $"GetAll{resourceName}sAsync",
            EndpointClassification.GetById => $"Get{resourceName}ByIdAsync",
            EndpointClassification.Create => $"Create{resourceName}Async",
            EndpointClassification.Update => $"Update{resourceName}Async",
            EndpointClassification.Delete => $"Delete{resourceName}Async",
            EndpointClassification.Search => $"Search{resourceName}sAsync",
            _ => $"{endpoint.Method}{resourceName}Async"
        };
    }

    private static string GetReturnType(ResolvedEndpoint endpoint)
    {
        if (endpoint.Classification == EndpointClassification.Delete)
        {
            return "Task";
        }

        string responseTypeName = !string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName)
            ? endpoint.ResponseSchemaName
            : "object";

        if (endpoint.Classification == EndpointClassification.List
            || endpoint.Classification == EndpointClassification.Search)
        {
            return $"Task<List<{responseTypeName}>>";
        }

        if (endpoint.Classification == EndpointClassification.Create
            || endpoint.Classification == EndpointClassification.Update
            || endpoint.Classification == EndpointClassification.GetById)
        {
            return $"Task<{responseTypeName}>";
        }

        return $"Task<{responseTypeName}>";
    }

    private static string BuildParameterList(ResolvedEndpoint endpoint)
    {
        List<string> parameters = new List<string>();

        foreach (CanisUIForge.OpenApi.Models.ParameterDefinition parameter in endpoint.Parameters)
        {
            string paramType = MapSchemaTypeToClrType(parameter.SchemaType);
            string paramName = ToCamelCase(parameter.Name);
            parameters.Add($"{paramType} {paramName}");
        }

        if (endpoint.Classification == EndpointClassification.Create
            || endpoint.Classification == EndpointClassification.Update)
        {
            string requestTypeName = !string.IsNullOrWhiteSpace(endpoint.RequestSchemaName)
                ? endpoint.RequestSchemaName
                : "object";

            parameters.Add($"{requestTypeName} request");
        }

        return string.Join(", ", parameters);
    }

    private static string BuildMethodBody(ResolvedEndpoint endpoint)
    {
        string route = BuildRouteExpression(endpoint);
        StringBuilder builder = new StringBuilder();

        switch (endpoint.Method)
        {
            case HttpMethodType.Get:
                string getResponseType = GetResponseTypeNameForBody(endpoint);
                if (endpoint.Classification == EndpointClassification.List
                    || endpoint.Classification == EndpointClassification.Search)
                {
                    builder.AppendLine($"        string requestUri = {route};");

                    if (endpoint.Classification == EndpointClassification.Search)
                    {
                        builder.AppendLine(BuildQueryStringAppend(endpoint));
                    }

                    builder.AppendLine($"        List<{getResponseType}>? result = await _httpClient.GetFromJsonAsync<List<{getResponseType}>>(requestUri, JsonOptions);");
                    builder.AppendLine($"        return result ?? new List<{getResponseType}>();");
                }
                else
                {
                    builder.AppendLine($"        string requestUri = {route};");
                    builder.AppendLine($"        {getResponseType}? result = await _httpClient.GetFromJsonAsync<{getResponseType}>(requestUri, JsonOptions);");
                    builder.AppendLine($"        return result ?? throw new InvalidOperationException(\"Response was null.\");");
                }
                break;

            case HttpMethodType.Post:
                string postResponseType = GetResponseTypeNameForBody(endpoint);
                builder.AppendLine($"        string requestUri = {route};");
                builder.AppendLine($"        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, request, JsonOptions);");
                builder.AppendLine($"        response.EnsureSuccessStatusCode();");
                builder.AppendLine($"        {postResponseType}? result = await response.Content.ReadFromJsonAsync<{postResponseType}>(JsonOptions);");
                builder.AppendLine($"        return result ?? throw new InvalidOperationException(\"Response was null.\");");
                break;

            case HttpMethodType.Put:
            case HttpMethodType.Patch:
                string putResponseType = GetResponseTypeNameForBody(endpoint);
                builder.AppendLine($"        string requestUri = {route};");
                builder.AppendLine($"        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(requestUri, request, JsonOptions);");
                builder.AppendLine($"        response.EnsureSuccessStatusCode();");
                builder.AppendLine($"        {putResponseType}? result = await response.Content.ReadFromJsonAsync<{putResponseType}>(JsonOptions);");
                builder.AppendLine($"        return result ?? throw new InvalidOperationException(\"Response was null.\");");
                break;

            case HttpMethodType.Delete:
                builder.AppendLine($"        string requestUri = {route};");
                builder.AppendLine($"        HttpResponseMessage response = await _httpClient.DeleteAsync(requestUri);");
                builder.AppendLine($"        response.EnsureSuccessStatusCode();");
                break;
        }

        return builder.ToString();
    }

    private static string BuildRouteExpression(ResolvedEndpoint endpoint)
    {
        string route = endpoint.Route;

        if (route.StartsWith("/", StringComparison.Ordinal))
        {
            route = route.Substring(1);
        }

        bool hasPathParameters = endpoint.Parameters.Any(
            parameter => string.Equals(parameter.Location, "path", StringComparison.OrdinalIgnoreCase));

        if (hasPathParameters)
        {
            foreach (CanisUIForge.OpenApi.Models.ParameterDefinition parameter in endpoint.Parameters)
            {
                if (string.Equals(parameter.Location, "path", StringComparison.OrdinalIgnoreCase))
                {
                    string paramName = ToCamelCase(parameter.Name);
                    route = route.Replace($"{{{parameter.Name}}}", $"{{{paramName}}}");
                }
            }

            return $"$\"{route}\"";
        }

        return $"\"{route}\"";
    }

    private static string BuildQueryStringAppend(ResolvedEndpoint endpoint)
    {
        List<CanisUIForge.OpenApi.Models.ParameterDefinition> queryParameters = endpoint.Parameters
            .Where(parameter => string.Equals(parameter.Location, "query", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (queryParameters.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("        List<string> queryParts = new List<string>();");

        foreach (CanisUIForge.OpenApi.Models.ParameterDefinition parameter in queryParameters)
        {
            string paramName = ToCamelCase(parameter.Name);
            string paramType = MapSchemaTypeToClrType(parameter.SchemaType);

            if (paramType == "string")
            {
                builder.AppendLine($"        if (!string.IsNullOrWhiteSpace({paramName}))");
            }
            else
            {
                builder.AppendLine($"        if ({paramName} != default)");
            }

            builder.AppendLine("        {");
            builder.AppendLine($"            queryParts.Add($\"{parameter.Name}={{{paramName}}}\");");
            builder.AppendLine("        }");
        }

        builder.AppendLine("        if (queryParts.Count > 0)");
        builder.AppendLine("        {");
        builder.AppendLine("            requestUri = requestUri + \"?\" + string.Join(\"&\", queryParts);");
        builder.Append("        }");

        return builder.ToString();
    }

    private static string GetResponseTypeNameForBody(ResolvedEndpoint endpoint)
    {
        return !string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName)
            ? endpoint.ResponseSchemaName
            : "object";
    }

    private static string MapSchemaTypeToClrType(string schemaType)
    {
        return schemaType.ToLowerInvariant() switch
        {
            "integer" or "int" or "int32" => "int",
            "int64" or "long" => "long",
            "number" or "double" => "double",
            "float" => "float",
            "decimal" => "decimal",
            "boolean" or "bool" => "bool",
            "string" => "string",
            "uuid" or "guid" => "Guid",
            _ => "string"
        };
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
