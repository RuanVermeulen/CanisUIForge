namespace CanisUIForge.IntegrationTests;

public class PlanBuildingTests : IAsyncLifetime
{
    private readonly TestServiceFactory _factory;
    private ApiDefinition _apiDefinition = null!;
    private ITypeRegistry _typeRegistry = null!;

    public PlanBuildingTests()
    {
        _factory = new TestServiceFactory();
    }

    public async Task InitializeAsync()
    {
        IOpenApiScanner scanner = _factory.CreateOpenApiScanner();
        _apiDefinition = await scanner.ScanAsync(TestPaths.GetSwaggerPath());

        IContractsResolver resolver = _factory.CreateContractsResolver();
        ContractsConfig contractsConfig = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = TestPaths.GetContractsProjectPath()
        };
        _typeRegistry = await resolver.ResolveAsync(contractsConfig);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void Build_WithBlazorTarget_CreatesValidPlan()
    {
        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        ForgeConfig config = CreateConfig(TargetPlatform.Blazor);

        GenerationPlan plan = planBuilder.Build(config, _apiDefinition, _typeRegistry);

        Assert.NotNull(plan);
        Assert.Equal("TestGenerated", plan.SolutionName);
        Assert.Contains(TargetPlatform.Blazor, plan.Targets);
        Assert.True(plan.Resources.Count > 0, "Plan should have at least one resource.");
    }

    [Fact]
    public void Build_WithControllersConfig_FiltersResources()
    {
        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        ForgeConfig config = CreateConfig(TargetPlatform.Blazor);
        config.Controllers.Add(new ControllerConfig { Name = "Customers", Style = GenerationStyle.FormAndGrid });
        config.Controllers.Add(new ControllerConfig { Name = "Products", Style = GenerationStyle.Grid });

        GenerationPlan plan = planBuilder.Build(config, _apiDefinition, _typeRegistry);

        List<string> resourceNames = plan.Resources.Select(r => r.Name).ToList();
        Assert.Contains("Customers", resourceNames);
        Assert.Contains("Products", resourceNames);
    }

    [Fact]
    public void Build_WithTestsEnabled_IncludesTestConfig()
    {
        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        ForgeConfig config = CreateConfig(TargetPlatform.Blazor);

        GenerationPlan plan = planBuilder.Build(config, _apiDefinition, _typeRegistry);

        Assert.True(plan.Tests.Unit);
        Assert.True(plan.Tests.Playwright);
        Assert.False(plan.Tests.Appium);
    }

    [Fact]
    public void Build_ResourcesHaveResolvedEndpoints()
    {
        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        ForgeConfig config = CreateConfig(TargetPlatform.Blazor);

        GenerationPlan plan = planBuilder.Build(config, _apiDefinition, _typeRegistry);

        foreach (ResolvedResource resource in plan.Resources)
        {
            Assert.True(resource.Endpoints.Count > 0, $"Resource '{resource.Name}' should have endpoints.");

            foreach (ResolvedEndpoint endpoint in resource.Endpoints)
            {
                Assert.False(string.IsNullOrWhiteSpace(endpoint.Route), $"Endpoint in '{resource.Name}' should have a route.");
            }
        }
    }

    [Fact]
    public void ValidateSchemaResolution_WithMatchingTypes_ReturnsValid()
    {
        IPipelineValidator pipelineValidator = _factory.CreatePipelineValidator();

        PipelineValidationResult result = pipelineValidator.ValidateSchemaResolution(_apiDefinition, _typeRegistry);

        Assert.True(result.IsValid, $"Schema validation failed. Errors: {string.Join(", ", result.Errors)}");
    }

    private static ForgeConfig CreateConfig(TargetPlatform target)
    {
        return new ForgeConfig
        {
            SolutionName = "TestGenerated",
            SwaggerSource = TestPaths.GetSwaggerPath(),
            OutputPath = TestPaths.CreateTempOutputDirectory(),
            NamespaceRoot = "TestGenerated",
            Targets = new List<TargetPlatform> { target },
            Contracts = new ContractsConfig
            {
                Mode = ContractsMode.ProjectReference,
                ProjectPath = TestPaths.GetContractsProjectPath()
            },
            Tests = new TestConfig
            {
                Unit = true,
                Playwright = true,
                Appium = false
            }
        };
    }
}
