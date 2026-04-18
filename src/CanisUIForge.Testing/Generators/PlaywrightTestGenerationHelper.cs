namespace CanisUIForge.Testing.Generators;

public static class PlaywrightTestGenerationHelper
{
    public static string BuildEditPageTests(ResolvedResource resource)
    {
        string resourceNameLower = resource.Name.ToLowerInvariant();
        bool hasGetById = resource.Endpoints.Any(e => e.Classification == EndpointClassification.GetById);
        bool hasUpdate = resource.Endpoints.Any(e => e.Classification == EndpointClassification.Update);

        if (!hasGetById && !hasUpdate)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();

        if (hasGetById)
        {
            builder.AppendLine("    [Fact]");
            builder.AppendLine($"    public async Task EditPage_Should_Render_Form()");
            builder.AppendLine("    {");
            builder.AppendLine($"        IPage page = await PlaywrightTestHelper.NavigateToPageAsync(_fixture, \"{resourceNameLower}/edit/1\");");
            builder.AppendLine();
            builder.AppendLine($"        await PlaywrightTestHelper.WaitForTestIdAsync(page, \"{resourceNameLower}-edit-header\");");
            builder.AppendLine($"        ILocator header = PlaywrightTestHelper.GetByTestId(page, \"{resourceNameLower}-edit-header\");");
            builder.AppendLine("        bool headerVisible = await header.IsVisibleAsync();");
            builder.AppendLine("        Assert.True(headerVisible, \"Edit page header should be visible.\");");
            builder.AppendLine("    }");
        }

        if (hasGetById && hasUpdate)
        {
            builder.AppendLine();
            builder.AppendLine("    [Fact]");
            builder.AppendLine($"    public async Task EditPage_Should_Have_SaveButton()");
            builder.AppendLine("    {");
            builder.AppendLine($"        IPage page = await PlaywrightTestHelper.NavigateToPageAsync(_fixture, \"{resourceNameLower}/edit/1\");");
            builder.AppendLine();
            builder.AppendLine($"        await PlaywrightTestHelper.WaitForTestIdAsync(page, \"submit-button\");");
            builder.AppendLine("        ILocator submitButton = PlaywrightTestHelper.GetByTestId(page, \"submit-button\");");
            builder.AppendLine("        bool isVisible = await submitButton.IsVisibleAsync();");
            builder.AppendLine("        Assert.True(isVisible, \"Save button should be visible on the edit page.\");");
            builder.AppendLine();
            builder.AppendLine("        string buttonText = await submitButton.InnerTextAsync();");
            builder.AppendLine("        Assert.Equal(\"Save\", buttonText);");
            builder.AppendLine("    }");
        }

        return builder.ToString().TrimEnd();
    }

    public static bool HasSearchEndpoint(ResolvedResource resource)
    {
        return resource.Endpoints.Any(e => e.Classification == EndpointClassification.Search);
    }

    public static bool HasListEndpoint(ResolvedResource resource)
    {
        return resource.Endpoints.Any(e => e.Classification == EndpointClassification.List);
    }

    public static bool HasCreateEndpoint(ResolvedResource resource)
    {
        return resource.Endpoints.Any(e => e.Classification == EndpointClassification.Create);
    }

    public static bool HasCrudEndpoints(ResolvedResource resource)
    {
        return HasCreateEndpoint(resource) ||
               resource.Endpoints.Any(e => e.Classification == EndpointClassification.Update) ||
               resource.Endpoints.Any(e => e.Classification == EndpointClassification.GetById);
    }
}
