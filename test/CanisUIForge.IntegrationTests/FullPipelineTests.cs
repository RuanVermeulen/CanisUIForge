namespace CanisUIForge.IntegrationTests;

public class FullPipelineTests : IAsyncLifetime, IDisposable
{
    private readonly TestServiceFactory _factory;
    private string _outputPath = string.Empty;

    public FullPipelineTests()
    {
        _factory = new TestServiceFactory();
    }

    public Task InitializeAsync()
    {
        _outputPath = TestPaths.CreateTempOutputDirectory();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        TestPaths.CleanupTempDirectory(_outputPath);
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesOutputFiles()
    {
        // Step 1: Scan Swagger
        IOpenApiScanner scanner = _factory.CreateOpenApiScanner();
        ApiDefinition apiDefinition = await scanner.ScanAsync(TestPaths.GetSwaggerPath());

        Assert.NotNull(apiDefinition);
        Assert.True(apiDefinition.Resources.Count >= 3);

        // Step 2: Resolve Contracts
        IContractsResolver contractsResolver = _factory.CreateContractsResolver();
        ContractsConfig contractsConfig = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = TestPaths.GetContractsProjectPath()
        };
        ITypeRegistry typeRegistry = await contractsResolver.ResolveAsync(contractsConfig);

        Assert.NotNull(typeRegistry);
        Assert.True(typeRegistry.GetAll().Count > 0);

        // Step 3: Validate
        IPipelineValidator pipelineValidator = _factory.CreatePipelineValidator();
        PipelineValidationResult schemaValidation = pipelineValidator.ValidateSchemaResolution(apiDefinition, typeRegistry);
        Assert.True(schemaValidation.IsValid, $"Schema validation failed: {string.Join(", ", schemaValidation.Errors)}");

        // Step 4: Build Plan
        ForgeConfig config = new ForgeConfig
        {
            SolutionName = "TestGenerated",
            SwaggerSource = TestPaths.GetSwaggerPath(),
            OutputPath = _outputPath,
            NamespaceRoot = "TestGenerated",
            Targets = new List<TargetPlatform> { TargetPlatform.Blazor },
            Contracts = contractsConfig,
            Controllers = new List<ControllerConfig>
            {
                new ControllerConfig { Name = "Customers", Style = GenerationStyle.FormAndGrid },
                new ControllerConfig { Name = "Products", Style = GenerationStyle.FormAndGrid },
                new ControllerConfig { Name = "Orders", Style = GenerationStyle.Grid }
            },
            Tests = new TestConfig
            {
                Unit = true,
                Playwright = true,
                Appium = false
            }
        };

        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        GenerationPlan plan = planBuilder.Build(config, apiDefinition, typeRegistry);

        Assert.NotNull(plan);
        Assert.True(plan.Resources.Count > 0);

        // Step 5: Execute Blazor Generation
        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        IBlazorComponentsGenerator blazorComponents = _factory.CreateBlazorComponentsGenerator();
        IBlazorApiServiceGenerator blazorApiServices = _factory.CreateBlazorApiServiceGenerator();
        IBlazorPageGenerator blazorPages = _factory.CreateBlazorPageGenerator();

        await blazorFoundation.GenerateAsync(plan);
        await blazorComponents.GenerateAsync(plan);
        await blazorApiServices.GenerateAsync(plan);
        await blazorPages.GenerateAsync(plan);

        // Step 6: Execute Test Generation
        IUnitTestGenerator unitTests = _factory.CreateUnitTestGenerator();
        IPlaywrightTestGenerator playwrightTests = _factory.CreatePlaywrightTestGenerator();

        await unitTests.GenerateAsync(plan);
        await playwrightTests.GenerateAsync(plan);

        // Step 7: Verify output
        string blazorProjectPath = Path.Combine(_outputPath, "TestGenerated.Blazor");
        Assert.True(Directory.Exists(blazorProjectPath), $"Blazor project directory should exist at {blazorProjectPath}");

        // Verify regeneration tracking
        RegenerationResult result = ((RegenerationTracker)_factory.Tracker).GetResult();
        Assert.True(result.TotalCount > 0, "Should have tracked file operations.");
        Assert.True(result.CreatedCount > 0, "Should have created files.");
        Assert.Equal(0, result.SkippedCount);
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesBlazorProjectFile()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        await blazorFoundation.GenerateAsync(plan);

        string blazorProjectPath = Path.Combine(_outputPath, "TestGenerated.Blazor");
        string csprojPath = Path.Combine(blazorProjectPath, "TestGenerated.Blazor.csproj");

        Assert.True(File.Exists(csprojPath), $"Blazor .csproj should exist at {csprojPath}");

        string csprojContent = await File.ReadAllTextAsync(csprojPath);
        Assert.Contains("net8.0", csprojContent);
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesApiServices()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        IBlazorApiServiceGenerator blazorApiServices = _factory.CreateBlazorApiServiceGenerator();

        await blazorFoundation.GenerateAsync(plan);
        await blazorApiServices.GenerateAsync(plan);

        string blazorProjectPath = Path.Combine(_outputPath, "TestGenerated.Blazor");
        string servicesPath = Path.Combine(blazorProjectPath, "Services");

        Assert.True(Directory.Exists(servicesPath), $"Services directory should exist at {servicesPath}");

        string[] serviceFiles = Directory.GetFiles(servicesPath, "*.cs");
        Assert.True(serviceFiles.Length > 0, "Should have generated service files.");
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesPages()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        IBlazorPageGenerator blazorPages = _factory.CreateBlazorPageGenerator();

        await blazorFoundation.GenerateAsync(plan);
        await blazorPages.GenerateAsync(plan);

        string blazorProjectPath = Path.Combine(_outputPath, "TestGenerated.Blazor");
        string pagesPath = Path.Combine(blazorProjectPath, "Pages");

        Assert.True(Directory.Exists(pagesPath), $"Pages directory should exist at {pagesPath}");

        string[] pageFiles = Directory.GetFiles(pagesPath, "*.razor", SearchOption.AllDirectories);
        Assert.True(pageFiles.Length > 0, "Should have generated page files.");
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesUnitTests()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IUnitTestGenerator unitTests = _factory.CreateUnitTestGenerator();
        await unitTests.GenerateAsync(plan);

        string testProjectPath = Path.Combine(_outputPath, "TestGenerated.Tests");
        Assert.True(Directory.Exists(testProjectPath), $"Test project directory should exist at {testProjectPath}");
    }

    [Fact]
    public async Task FullBlazorPipeline_GeneratesPlaywrightTests()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IPlaywrightTestGenerator playwrightTests = _factory.CreatePlaywrightTestGenerator();
        await playwrightTests.GenerateAsync(plan);

        string playwrightPath = Path.Combine(_outputPath, "TestGenerated.Playwright");
        Assert.True(Directory.Exists(playwrightPath), $"Playwright project directory should exist at {playwrightPath}");
    }

    [Fact]
    public async Task RegenerationTracking_RecordsAllCreatedFiles()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        await blazorFoundation.GenerateAsync(plan);

        RegenerationResult result = ((RegenerationTracker)_factory.Tracker).GetResult();

        Assert.True(result.CreatedCount > 0, "Should have created at least one file.");
        Assert.Equal(0, result.OverwrittenCount);
        Assert.Equal(0, result.SkippedCount);
    }

    [Fact]
    public async Task Regeneration_SecondRun_OverwritesGeneratedFiles()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();

        // First run
        await blazorFoundation.GenerateAsync(plan);

        RegenerationResult firstResult = ((RegenerationTracker)_factory.Tracker).GetResult();
        int firstCreatedCount = firstResult.CreatedCount;
        Assert.True(firstCreatedCount > 0);

        // Reset tracker
        ((RegenerationTracker)_factory.Tracker).Reset();

        // Second run - same files should be overwritten
        await blazorFoundation.GenerateAsync(plan);

        RegenerationResult secondResult = ((RegenerationTracker)_factory.Tracker).GetResult();
        Assert.True(secondResult.OverwrittenCount > 0, "Second run should overwrite previously generated files.");
        Assert.Equal(0, secondResult.CreatedCount);
    }

    [Fact]
    public async Task GeneratedFiles_HaveAutoGeneratedHeader()
    {
        GenerationPlan plan = await BuildPlan(TargetPlatform.Blazor);

        IBlazorFoundationGenerator blazorFoundation = _factory.CreateBlazorFoundationGenerator();
        await blazorFoundation.GenerateAsync(plan);

        string blazorProjectPath = Path.Combine(_outputPath, "TestGenerated.Blazor");
        string[] csFiles = Directory.GetFiles(blazorProjectPath, "*.cs", SearchOption.AllDirectories);

        foreach (string csFile in csFiles)
        {
            Assert.True(CanisUIForge.Generation.Output.GeneratedFileHeader.IsGeneratedFile(csFile),
                $"File '{Path.GetFileName(csFile)}' should have auto-generated header.");
        }
    }

    private async Task<GenerationPlan> BuildPlan(TargetPlatform target)
    {
        IOpenApiScanner scanner = _factory.CreateOpenApiScanner();
        ApiDefinition apiDefinition = await scanner.ScanAsync(TestPaths.GetSwaggerPath());

        IContractsResolver contractsResolver = _factory.CreateContractsResolver();
        ContractsConfig contractsConfig = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = TestPaths.GetContractsProjectPath()
        };
        ITypeRegistry typeRegistry = await contractsResolver.ResolveAsync(contractsConfig);

        ForgeConfig config = new ForgeConfig
        {
            SolutionName = "TestGenerated",
            SwaggerSource = TestPaths.GetSwaggerPath(),
            OutputPath = _outputPath,
            NamespaceRoot = "TestGenerated",
            Targets = new List<TargetPlatform> { target },
            Contracts = contractsConfig,
            Controllers = new List<ControllerConfig>
            {
                new ControllerConfig { Name = "Customers", Style = GenerationStyle.FormAndGrid },
                new ControllerConfig { Name = "Products", Style = GenerationStyle.FormAndGrid },
                new ControllerConfig { Name = "Orders", Style = GenerationStyle.Grid }
            },
            Tests = new TestConfig
            {
                Unit = true,
                Playwright = true,
                Appium = false
            }
        };

        IGenerationPlanBuilder planBuilder = _factory.CreatePlanBuilder();
        return planBuilder.Build(config, apiDefinition, typeRegistry);
    }
}
