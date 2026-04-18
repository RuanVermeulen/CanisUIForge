namespace CanisUIForge.Generation.Planning;

public class GenerationPlanBuilder : IGenerationPlanBuilder
{
    private readonly IMetadataUnifier _metadataUnifier;

    public GenerationPlanBuilder(IMetadataUnifier metadataUnifier)
    {
        _metadataUnifier = metadataUnifier ?? throw new ArgumentNullException(nameof(metadataUnifier));
    }

    public GenerationPlan Build(ForgeConfig config, ApiDefinition apiDefinition, ITypeRegistry typeRegistry)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (apiDefinition is null)
        {
            throw new ArgumentNullException(nameof(apiDefinition));
        }

        if (typeRegistry is null)
        {
            throw new ArgumentNullException(nameof(typeRegistry));
        }

        List<ResolvedResource> resolvedResources = _metadataUnifier.Unify(apiDefinition, typeRegistry);

        List<ResolvedResource> filteredResources = FilterAndApplyStyles(resolvedResources, config.Controllers);

        string namespaceRoot = !string.IsNullOrWhiteSpace(config.NamespaceRoot)
            ? config.NamespaceRoot
            : config.SolutionName;

        GenerationPlan plan = new GenerationPlan
        {
            SolutionName = config.SolutionName,
            NamespaceRoot = namespaceRoot,
            OutputPath = config.OutputPath,
            Targets = config.Targets,
            Resources = filteredResources,
            Tests = config.Tests,
            ApiTitle = apiDefinition.Title,
            ApiVersion = apiDefinition.Version
        };

        return plan;
    }

    private static List<ResolvedResource> FilterAndApplyStyles(
        List<ResolvedResource> resources,
        List<ControllerConfig> controllerConfigs)
    {
        if (controllerConfigs.Count == 0)
        {
            return resources;
        }

        Dictionary<string, ControllerConfig> controllerLookup = controllerConfigs
            .ToDictionary(
                controller => controller.Name,
                controller => controller,
                StringComparer.OrdinalIgnoreCase);

        List<ResolvedResource> filteredResources = new List<ResolvedResource>();

        foreach (ResolvedResource resource in resources)
        {
            if (controllerLookup.TryGetValue(resource.Name, out ControllerConfig? controllerConfig))
            {
                resource.Style = controllerConfig.Style;
                filteredResources.Add(resource);
            }
        }

        return filteredResources;
    }
}
