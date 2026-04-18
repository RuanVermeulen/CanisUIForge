namespace CanisUIForge.Blazor.Generators;

public static class PageGenerationHelper
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

    public static string GetIdRouteConstraint(Type? responseType)
    {
        string typeName = GetIdPropertyTypeName(responseType);

        return typeName switch
        {
            "int" => ":int",
            "long" => ":long",
            "Guid" => ":guid",
            "bool" => ":bool",
            _ => string.Empty
        };
    }

    public static string BuildGridColumnInitializers(string responseTypeName, Type? responseType)
    {
        if (responseType is null)
        {
            return $"            // TODO: Add grid columns for {responseTypeName}";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = responseType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int index = 0; index < properties.Length; index++)
        {
            PropertyInfo property = properties[index];
            string separator = index < properties.Length - 1 ? "," : string.Empty;
            builder.AppendLine($"            new DataGridColumn<{responseTypeName}> {{ Title = \"{property.Name}\", ValueSelector = item => item.{property.Name} }}{separator}");
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildFormFieldRenderers(Type? requestType)
    {
        if (requestType is null)
        {
            return "        @* TODO: Add form fields *@";
        }

        StringBuilder builder = new StringBuilder();
        PropertyInfo[] properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            string fieldType = MapClrTypeToFieldType(property.PropertyType);
            string label = property.Name;
            string fieldId = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
            string valueCast = BuildValueCast(property.PropertyType);

            builder.AppendLine($"        <FieldRenderer Label=\"{label}\" FieldId=\"{fieldId}\" FieldType=\"{fieldType}\"");
            builder.AppendLine($"                       Value=\"@model.{property.Name}\"");
            builder.AppendLine($"                       ValueChanged=\"@(value => model.{property.Name} = {valueCast})\" />");
        }

        return builder.ToString().TrimEnd();
    }

    private static string MapClrTypeToFieldType(Type type)
    {
        Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType == typeof(bool)) return "bool";
        if (underlyingType == typeof(int) || underlyingType == typeof(long)) return "int";
        if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float)) return "decimal";
        if (underlyingType == typeof(DateTime)) return "datetime";
        if (underlyingType == typeof(DateTimeOffset)) return "datetimeoffset";
        if (underlyingType == typeof(DateOnly)) return "date";

        return "string";
    }

    private static string BuildValueCast(Type propertyType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlyingType == typeof(string))
            return "value?.ToString() ?? string.Empty";
        if (underlyingType == typeof(bool))
            return "value is bool boolValue ? boolValue : false";
        if (underlyingType == typeof(int))
            return "int.TryParse(value?.ToString(), out int intValue) ? intValue : 0";
        if (underlyingType == typeof(long))
            return "long.TryParse(value?.ToString(), out long longValue) ? longValue : 0L";
        if (underlyingType == typeof(decimal))
            return "decimal.TryParse(value?.ToString(), out decimal decimalValue) ? decimalValue : 0m";
        if (underlyingType == typeof(double))
            return "double.TryParse(value?.ToString(), out double doubleValue) ? doubleValue : 0d";
        if (underlyingType == typeof(float))
            return "float.TryParse(value?.ToString(), out float floatValue) ? floatValue : 0f";
        if (underlyingType == typeof(DateTime))
            return "DateTime.TryParse(value?.ToString(), out DateTime dateTimeValue) ? dateTimeValue : DateTime.MinValue";
        if (underlyingType == typeof(DateTimeOffset))
            return "DateTimeOffset.TryParse(value?.ToString(), out DateTimeOffset dateTimeOffsetValue) ? dateTimeOffsetValue : DateTimeOffset.MinValue";
        if (underlyingType == typeof(Guid))
            return "Guid.TryParse(value?.ToString(), out Guid guidValue) ? guidValue : Guid.Empty";

        return "value?.ToString() ?? string.Empty";
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
}
