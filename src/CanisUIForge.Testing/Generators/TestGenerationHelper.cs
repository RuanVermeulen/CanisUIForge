namespace CanisUIForge.Testing.Generators;

public static class TestGenerationHelper
{
    public static string BuildTestMethods(ResolvedResource resource)
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
            string testMethod = BuildTestMethod(endpoint, resource.Name);
            builder.Append(testMethod);
        }

        return builder.ToString().TrimEnd();
    }

    private static string BuildTestMethod(ResolvedEndpoint endpoint, string resourceName)
    {
        string methodName = GetServiceMethodName(endpoint, resourceName);
        string testName = $"{methodName}_ReturnsExpectedResult";

        return endpoint.Classification switch
        {
            EndpointClassification.List => BuildListTestMethod(testName, methodName, endpoint, resourceName),
            EndpointClassification.GetById => BuildGetByIdTestMethod(testName, methodName, endpoint, resourceName),
            EndpointClassification.Create => BuildCreateTestMethod(testName, methodName, endpoint, resourceName),
            EndpointClassification.Update => BuildUpdateTestMethod(testName, methodName, endpoint, resourceName),
            EndpointClassification.Delete => BuildDeleteTestMethod(testName, methodName, endpoint, resourceName),
            EndpointClassification.Search => BuildSearchTestMethod(testName, methodName, endpoint, resourceName),
            _ => BuildGenericTestMethod(testName, methodName, endpoint, resourceName)
        };
    }

    private static string BuildListTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string responseType = GetResponseTypeName(endpoint);
        string route = NormalizeRoute(endpoint.Route);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        List<{responseType}> expected = new List<{responseType}>();");
        builder.AppendLine($"        MessageHandler.SetupJsonResponse(\"{route}\", expected);");
        builder.AppendLine();
        builder.AppendLine($"        List<{responseType}> result = await _service.{methodName}();");
        builder.AppendLine();
        builder.AppendLine("        Assert.NotNull(result);");
        builder.AppendLine("        Assert.Equal(expected.Count, result.Count);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildGetByIdTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string responseType = GetResponseTypeName(endpoint);
        string route = NormalizeRoute(endpoint.Route);
        string testRoute = BuildTestRoute(route);
        string paramArgs = BuildTestParameterArguments(endpoint);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        {responseType} expected = new {responseType}();");
        builder.AppendLine($"        MessageHandler.SetupJsonResponse(\"{testRoute}\", expected);");
        builder.AppendLine();
        builder.AppendLine($"        {responseType} result = await _service.{methodName}({paramArgs});");
        builder.AppendLine();
        builder.AppendLine("        Assert.NotNull(result);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildCreateTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string responseType = GetResponseTypeName(endpoint);
        string requestType = GetRequestTypeName(endpoint);
        string route = NormalizeRoute(endpoint.Route);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        {requestType} request = new {requestType}();");
        builder.AppendLine($"        {responseType} expected = new {responseType}();");
        builder.AppendLine($"        MessageHandler.SetupJsonResponse(\"{route}\", expected);");
        builder.AppendLine();
        builder.AppendLine($"        {responseType} result = await _service.{methodName}(request);");
        builder.AppendLine();
        builder.AppendLine("        Assert.NotNull(result);");
        builder.AppendLine("        Assert.Single(MessageHandler.Requests);");
        builder.AppendLine("        Assert.Equal(HttpMethod.Post, MessageHandler.Requests[0].Method);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildUpdateTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string responseType = GetResponseTypeName(endpoint);
        string requestType = GetRequestTypeName(endpoint);
        string route = NormalizeRoute(endpoint.Route);
        string testRoute = BuildTestRoute(route);
        string paramArgs = BuildTestParameterArguments(endpoint);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        {requestType} request = new {requestType}();");
        builder.AppendLine($"        {responseType} expected = new {responseType}();");
        builder.AppendLine($"        MessageHandler.SetupJsonResponse(\"{testRoute}\", expected);");
        builder.AppendLine();
        builder.AppendLine($"        {responseType} result = await _service.{methodName}({paramArgs}, request);");
        builder.AppendLine();
        builder.AppendLine("        Assert.NotNull(result);");
        builder.AppendLine("        Assert.Single(MessageHandler.Requests);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildDeleteTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string route = NormalizeRoute(endpoint.Route);
        string testRoute = BuildTestRoute(route);
        string paramArgs = BuildTestParameterArguments(endpoint);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        MessageHandler.SetupEmptyResponse(\"{testRoute}\");");
        builder.AppendLine();
        builder.AppendLine($"        await _service.{methodName}({paramArgs});");
        builder.AppendLine();
        builder.AppendLine("        Assert.Single(MessageHandler.Requests);");
        builder.AppendLine("        Assert.Equal(HttpMethod.Delete, MessageHandler.Requests[0].Method);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildSearchTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        string responseType = GetResponseTypeName(endpoint);
        string route = NormalizeRoute(endpoint.Route);
        string paramArgs = BuildTestSearchArguments(endpoint);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        List<{responseType}> expected = new List<{responseType}>();");
        builder.AppendLine($"        MessageHandler.SetupJsonResponse(\"{route}\", expected);");
        builder.AppendLine();
        builder.AppendLine($"        List<{responseType}> result = await _service.{methodName}({paramArgs});");
        builder.AppendLine();
        builder.AppendLine("        Assert.NotNull(result);");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string BuildGenericTestMethod(
        string testName,
        string methodName,
        ResolvedEndpoint endpoint,
        string resourceName)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("    [Fact]");
        builder.AppendLine($"    public async Task {testName}()");
        builder.AppendLine("    {");
        builder.AppendLine($"        // TODO: Add test implementation for {methodName}");
        builder.AppendLine("        await Task.CompletedTask;");
        builder.AppendLine("    }");

        return builder.ToString();
    }

    private static string GetServiceMethodName(ResolvedEndpoint endpoint, string resourceName)
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

    private static string GetResponseTypeName(ResolvedEndpoint endpoint)
    {
        return !string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName)
            ? endpoint.ResponseSchemaName
            : "object";
    }

    private static string GetRequestTypeName(ResolvedEndpoint endpoint)
    {
        return !string.IsNullOrWhiteSpace(endpoint.RequestSchemaName)
            ? endpoint.RequestSchemaName
            : "object";
    }

    private static string NormalizeRoute(string route)
    {
        if (route.StartsWith("/", StringComparison.Ordinal))
        {
            return route.Substring(1);
        }

        return route;
    }

    private static string BuildTestRoute(string route)
    {
        string result = route;

        foreach (ParameterDefinition parameter in GetPathParameterNames(route))
        {
            result = result.Replace($"{{{parameter.Name}}}", GetDefaultTestValue(parameter.SchemaType));
        }

        return result;
    }

    private static string BuildTestParameterArguments(ResolvedEndpoint endpoint)
    {
        List<string> arguments = new List<string>();

        foreach (ParameterDefinition parameter in endpoint.Parameters)
        {
            if (string.Equals(parameter.Location, "path", StringComparison.OrdinalIgnoreCase))
            {
                arguments.Add(GetDefaultTestLiteral(parameter.SchemaType));
            }
        }

        return string.Join(", ", arguments);
    }

    private static string BuildTestSearchArguments(ResolvedEndpoint endpoint)
    {
        List<string> arguments = new List<string>();

        foreach (ParameterDefinition parameter in endpoint.Parameters)
        {
            arguments.Add(GetDefaultTestLiteral(parameter.SchemaType));
        }

        if (arguments.Count == 0)
        {
            arguments.Add("\"test\"");
        }

        return string.Join(", ", arguments);
    }

    private static List<ParameterDefinition> GetPathParameterNames(string route)
    {
        List<ParameterDefinition> parameters = new List<ParameterDefinition>();
        int startIndex = 0;

        while (true)
        {
            int openBrace = route.IndexOf('{', startIndex);
            if (openBrace < 0)
            {
                break;
            }

            int closeBrace = route.IndexOf('}', openBrace);
            if (closeBrace < 0)
            {
                break;
            }

            string paramName = route.Substring(openBrace + 1, closeBrace - openBrace - 1);
            parameters.Add(new ParameterDefinition { Name = paramName, Location = "path", SchemaType = "string" });
            startIndex = closeBrace + 1;
        }

        return parameters;
    }

    private static string GetDefaultTestValue(string schemaType)
    {
        return schemaType.ToLowerInvariant() switch
        {
            "integer" or "int" or "int32" or "int64" or "long" => "1",
            "guid" or "uuid" => "00000000-0000-0000-0000-000000000001",
            _ => "test"
        };
    }

    private static string GetDefaultTestLiteral(string schemaType)
    {
        return schemaType.ToLowerInvariant() switch
        {
            "integer" or "int" or "int32" or "int64" or "long" => "1",
            "guid" or "uuid" => "Guid.Parse(\"00000000-0000-0000-0000-000000000001\")",
            "boolean" or "bool" => "true",
            _ => "\"test\""
        };
    }
}
