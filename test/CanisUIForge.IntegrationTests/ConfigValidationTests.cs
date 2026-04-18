namespace CanisUIForge.IntegrationTests;

public class ConfigValidationTests
{
    private readonly IConfigValidator _validator;

    public ConfigValidationTests()
    {
        _validator = new ConfigValidator();
    }

    [Fact]
    public void Validate_WithValidBlazorConfig_ReturnsValid()
    {
        ForgeConfig config = CreateValidConfig();

        ConfigValidationResult result = _validator.Validate(config);

        Assert.True(result.IsValid, $"Expected valid config. Errors: {string.Join(", ", result.Errors)}");
    }

    [Fact]
    public void Validate_WithMissingSolutionName_ReturnsInvalid()
    {
        ForgeConfig config = CreateValidConfig();
        config.SolutionName = string.Empty;

        ConfigValidationResult result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("SolutionName"));
    }

    [Fact]
    public void Validate_WithNoTargets_ReturnsInvalid()
    {
        ForgeConfig config = CreateValidConfig();
        config.Targets.Clear();

        ConfigValidationResult result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("target"));
    }

    [Fact]
    public void Validate_WithMissingSwaggerSource_ReturnsInvalid()
    {
        ForgeConfig config = CreateValidConfig();
        config.SwaggerSource = string.Empty;

        ConfigValidationResult result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("SwaggerSource"));
    }

    [Fact]
    public void Validate_WithProjectReferenceButMissingPath_ReturnsInvalid()
    {
        ForgeConfig config = CreateValidConfig();
        config.Contracts.Mode = ContractsMode.ProjectReference;
        config.Contracts.ProjectPath = string.Empty;

        ConfigValidationResult result = _validator.Validate(config);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("ProjectPath"));
    }

    [Fact]
    public void Validate_WithControllerNames_ReturnsValid()
    {
        ForgeConfig config = CreateValidConfig();
        config.Controllers.Add(new ControllerConfig { Name = "Customers", Style = GenerationStyle.FormAndGrid });
        config.Controllers.Add(new ControllerConfig { Name = "Products", Style = GenerationStyle.Grid });

        ConfigValidationResult result = _validator.Validate(config);

        Assert.True(result.IsValid, $"Expected valid config. Errors: {string.Join(", ", result.Errors)}");
    }

    private static ForgeConfig CreateValidConfig()
    {
        return new ForgeConfig
        {
            SolutionName = "TestGenerated",
            SwaggerSource = TestPaths.GetSwaggerPath(),
            OutputPath = Path.GetTempPath(),
            NamespaceRoot = "TestGenerated",
            Targets = new List<TargetPlatform> { TargetPlatform.Blazor },
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
