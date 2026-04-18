namespace CanisUIForge.Maui.Generators;

public static class MauiPageGenerationHelper
{
    public static ResolvedEndpoint? FindEndpoint(ResolvedResource resource, EndpointClassification classification)
    {
        return resource.Endpoints.FirstOrDefault(endpoint => endpoint.Classification == classification);
    }

    public static string GetResponseTypeName(ResolvedEndpoint? endpoint, string resourceName)
    {
        if (endpoint is not null && !string.IsNullOrWhiteSpace(endpoint.ResponseSchemaName))
        {
            return endpoint.ResponseSchemaName;
        }

        return $"{resourceName}Response";
    }

    public static string GetRequestTypeName(ResolvedEndpoint? endpoint, string resourceName, string prefix)
    {
        if (endpoint is not null && !string.IsNullOrWhiteSpace(endpoint.RequestSchemaName))
        {
            return endpoint.RequestSchemaName;
        }

        return $"{prefix}{resourceName}Request";
    }

    public static string GetIdPropertyName(Type? responseType)
    {
        if (responseType is null)
        {
            return "Id";
        }

        PropertyInfo? idProperty = responseType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

        if (idProperty is null)
        {
            idProperty = responseType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(property => property.Name.EndsWith("Id", StringComparison.Ordinal));
        }

        return idProperty?.Name ?? "Id";
    }

    public static string GetIdPropertyTypeName(Type? responseType)
    {
        if (responseType is null)
        {
            return "int";
        }

        string idPropertyName = GetIdPropertyName(responseType);
        PropertyInfo? idProperty = responseType.GetProperty(idPropertyName, BindingFlags.Public | BindingFlags.Instance);

        if (idProperty is null)
        {
            return "int";
        }

        return GetFriendlyTypeName(idProperty.PropertyType);
    }

    public static string GetIdParseExpression(Type? responseType)
    {
        string typeName = GetIdPropertyTypeName(responseType);

        return typeName switch
        {
            "int" => "int.TryParse(value, out int parsed) ? parsed : 0",
            "long" => "long.TryParse(value, out long parsed) ? parsed : 0L",
            "Guid" => "Guid.TryParse(value, out Guid parsed) ? parsed : Guid.Empty",
            _ => "value"
        };
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

    public static string BuildItemTemplateContent(Type? responseType)
    {
        if (responseType is null)
        {
            return "                                    <!-- TODO: Add item template bindings -->";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = responseType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int index = 0; index < properties.Length; index++)
        {
            PropertyInfo property = properties[index];

            if (index == 0)
            {
                builder.AppendLine($"                                    <Label Text=\"{{{{Binding {property.Name}}}}}\" FontAttributes=\"Bold\" FontSize=\"14\" />");
            }
            else
            {
                builder.AppendLine($"                                    <Label Text=\"{{{{Binding {property.Name}}}}}\" TextColor=\"{{{{StaticResource TextSecondary}}}}\" FontSize=\"12\" />");
            }
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildFormFields(Type? requestType)
    {
        if (requestType is null)
        {
            return "                <!-- TODO: Add form fields -->";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            string fieldId = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);

            if (underlyingType == typeof(bool))
            {
                builder.AppendLine($"                <HorizontalStackLayout Spacing=\"10\">");
                builder.AppendLine($"                    <Label Text=\"{property.Name}\" Style=\"{{{{StaticResource CaptionStyle}}}}\" VerticalOptions=\"Center\" />");
                builder.AppendLine($"                    <Switch x:Name=\"{property.Name}Switch\" AutomationId=\"{fieldId}-switch\" />");
                builder.AppendLine($"                </HorizontalStackLayout>");
            }
            else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateOnly))
            {
                builder.AppendLine($"                <VerticalStackLayout Spacing=\"2\">");
                builder.AppendLine($"                    <Label Text=\"{property.Name}\" Style=\"{{{{StaticResource CaptionStyle}}}}\" />");
                builder.AppendLine($"                    <DatePicker x:Name=\"{property.Name}Picker\" AutomationId=\"{fieldId}-picker\" />");
                builder.AppendLine($"                </VerticalStackLayout>");
            }
            else if (underlyingType == typeof(DateTimeOffset))
            {
                builder.AppendLine($"                <VerticalStackLayout Spacing=\"2\">");
                builder.AppendLine($"                    <Label Text=\"{property.Name}\" Style=\"{{{{StaticResource CaptionStyle}}}}\" />");
                builder.AppendLine($"                    <DatePicker x:Name=\"{property.Name}Picker\" AutomationId=\"{fieldId}-picker\" />");
                builder.AppendLine($"                </VerticalStackLayout>");
            }
            else
            {
                string keyboard = GetKeyboard(underlyingType);
                builder.AppendLine($"                <VerticalStackLayout Spacing=\"2\">");
                builder.AppendLine($"                    <Label Text=\"{property.Name}\" Style=\"{{{{StaticResource CaptionStyle}}}}\" />");
                builder.AppendLine($"                    <Entry x:Name=\"{property.Name}Entry\" Placeholder=\"{property.Name}\"{keyboard} AutomationId=\"{fieldId}-entry\" />");
                builder.AppendLine($"                </VerticalStackLayout>");
            }
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildFormFieldAssignments(Type? requestType)
    {
        if (requestType is null)
        {
            return "            // TODO: Assign form field values to model";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (underlyingType == typeof(string))
            {
                builder.AppendLine($"            model.{property.Name} = {property.Name}Entry.Text ?? string.Empty;");
            }
            else if (underlyingType == typeof(bool))
            {
                builder.AppendLine($"            model.{property.Name} = {property.Name}Switch.IsToggled;");
            }
            else if (underlyingType == typeof(int))
            {
                builder.AppendLine($"            model.{property.Name} = int.TryParse({property.Name}Entry.Text, out int {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : 0;");
            }
            else if (underlyingType == typeof(long))
            {
                builder.AppendLine($"            model.{property.Name} = long.TryParse({property.Name}Entry.Text, out long {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : 0L;");
            }
            else if (underlyingType == typeof(decimal))
            {
                builder.AppendLine($"            model.{property.Name} = decimal.TryParse({property.Name}Entry.Text, out decimal {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : 0m;");
            }
            else if (underlyingType == typeof(double))
            {
                builder.AppendLine($"            model.{property.Name} = double.TryParse({property.Name}Entry.Text, out double {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : 0d;");
            }
            else if (underlyingType == typeof(float))
            {
                builder.AppendLine($"            model.{property.Name} = float.TryParse({property.Name}Entry.Text, out float {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : 0f;");
            }
            else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateOnly))
            {
                builder.AppendLine($"            model.{property.Name} = {property.Name}Picker.Date;");
            }
            else if (underlyingType == typeof(DateTimeOffset))
            {
                builder.AppendLine($"            model.{property.Name} = new DateTimeOffset({property.Name}Picker.Date);");
            }
            else if (underlyingType == typeof(Guid))
            {
                builder.AppendLine($"            model.{property.Name} = Guid.TryParse({property.Name}Entry.Text, out Guid {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value) ? {char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}Value : Guid.Empty;");
            }
            else
            {
                builder.AppendLine($"            model.{property.Name} = {property.Name}Entry.Text ?? string.Empty;");
            }
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildFormFieldPopulation(Type? requestType)
    {
        if (requestType is null)
        {
            return "            // TODO: Populate form fields from result";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (underlyingType == typeof(bool))
            {
                builder.AppendLine($"            {property.Name}Switch.IsToggled = result.{property.Name};");
            }
            else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateOnly))
            {
                builder.AppendLine($"            {property.Name}Picker.Date = result.{property.Name};");
            }
            else if (underlyingType == typeof(DateTimeOffset))
            {
                builder.AppendLine($"            {property.Name}Picker.Date = result.{property.Name}.DateTime;");
            }
            else
            {
                builder.AppendLine($"            {property.Name}Entry.Text = result.{property.Name}?.ToString() ?? string.Empty;");
            }
        }

        return builder.ToString().TrimEnd();
    }

    private static string GetFriendlyTypeName(Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(int)) return "int";
        if (underlyingType == typeof(long)) return "long";
        if (underlyingType == typeof(string)) return "string";
        if (underlyingType == typeof(bool)) return "bool";
        if (underlyingType == typeof(decimal)) return "decimal";
        if (underlyingType == typeof(double)) return "double";
        if (underlyingType == typeof(float)) return "float";
        if (underlyingType == typeof(Guid)) return "Guid";

        return underlyingType.Name;
    }

    private static string GetKeyboard(Type underlyingType)
    {
        if (underlyingType == typeof(int) || underlyingType == typeof(long)
            || underlyingType == typeof(decimal) || underlyingType == typeof(double)
            || underlyingType == typeof(float))
        {
            return " Keyboard=\"Numeric\"";
        }

        return string.Empty;
    }
}
