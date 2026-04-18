namespace CanisUIForge.IntegrationTests;

public class SwaggerScanningTests : IDisposable
{
    private readonly TestServiceFactory _factory;
    private readonly IOpenApiScanner _scanner;

    public SwaggerScanningTests()
    {
        _factory = new TestServiceFactory();
        _scanner = _factory.CreateOpenApiScanner();
    }

    [Fact]
    public async Task ScanAsync_WithTestSwagger_ReturnsApiDefinition()
    {
        string swaggerPath = TestPaths.GetSwaggerPath();

        ApiDefinition result = await _scanner.ScanAsync(swaggerPath);

        Assert.NotNull(result);
        Assert.Equal("TestApi", result.Title);
        Assert.Equal("v1", result.Version);
    }

    [Fact]
    public async Task ScanAsync_WithTestSwagger_FindsAllResources()
    {
        string swaggerPath = TestPaths.GetSwaggerPath();

        ApiDefinition result = await _scanner.ScanAsync(swaggerPath);

        Assert.True(result.Resources.Count >= 3, $"Expected at least 3 resources, found {result.Resources.Count}");

        List<string> resourceNames = result.Resources.Select(r => r.Name).ToList();
        Assert.Contains("Customers", resourceNames);
        Assert.Contains("Products", resourceNames);
        Assert.Contains("Orders", resourceNames);
    }

    [Fact]
    public async Task ScanAsync_WithTestSwagger_ClassifiesCustomerEndpoints()
    {
        string swaggerPath = TestPaths.GetSwaggerPath();

        ApiDefinition result = await _scanner.ScanAsync(swaggerPath);

        ResourceDefinition? customers = result.Resources.FirstOrDefault(r => r.Name == "Customers");
        Assert.NotNull(customers);

        List<EndpointClassification> classifications = customers.Endpoints
            .Select(e => e.Classification)
            .ToList();

        Assert.Contains(EndpointClassification.List, classifications);
        Assert.Contains(EndpointClassification.GetById, classifications);
        Assert.Contains(EndpointClassification.Create, classifications);
        Assert.Contains(EndpointClassification.Update, classifications);
        Assert.Contains(EndpointClassification.Delete, classifications);
        Assert.Contains(EndpointClassification.Search, classifications);
    }

    [Fact]
    public async Task ScanAsync_WithTestSwagger_DetectsRequestAndResponseSchemas()
    {
        string swaggerPath = TestPaths.GetSwaggerPath();

        ApiDefinition result = await _scanner.ScanAsync(swaggerPath);

        ResourceDefinition? customers = result.Resources.FirstOrDefault(r => r.Name == "Customers");
        Assert.NotNull(customers);

        EndpointDefinition? createEndpoint = customers.Endpoints
            .FirstOrDefault(e => e.Classification == EndpointClassification.Create);
        Assert.NotNull(createEndpoint);
        Assert.Equal("CreateCustomerRequest", createEndpoint.RequestSchemaName);
        Assert.Equal("CustomerResponse", createEndpoint.ResponseSchemaName);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
