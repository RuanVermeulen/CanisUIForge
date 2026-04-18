namespace CanisUIForge.Maui.Generators;

public static class MauiServiceRegistrationHelper
{
    public static string BuildServiceRegistrations(List<ResolvedResource> resources, string namespaceRoot)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ResolvedResource resource in resources)
        {
            builder.AppendLine($"        builder.Services.AddSingleton<I{resource.Name}ApiService, {resource.Name}ApiService>();");
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildShellItems(List<ResolvedResource> resources, string namespaceRoot)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ResolvedResource resource in resources)
        {
            string resourceNameLower = resource.Name.ToLowerInvariant();
            builder.AppendLine($"    <ShellContent");
            builder.AppendLine($"        Title=\"{resource.Name}\"");
            builder.AppendLine($"        ContentTemplate=\"{{DataTemplate pages:{resource.Name}ListPage}}\"");
            builder.AppendLine($"        Route=\"{resourceNameLower}\" />");
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    public static string BuildRouteRegistrations(List<ResolvedResource> resources, string namespaceRoot)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ResolvedResource resource in resources)
        {
            string resourceNameLower = resource.Name.ToLowerInvariant();
            builder.AppendLine($"        Routing.RegisterRoute(\"{resourceNameLower}/create\", typeof(Pages.{resource.Name}CreatePage));");
            builder.AppendLine($"        Routing.RegisterRoute(\"{resourceNameLower}/edit\", typeof(Pages.{resource.Name}EditPage));");
        }

        return builder.ToString().TrimEnd();
    }
}
