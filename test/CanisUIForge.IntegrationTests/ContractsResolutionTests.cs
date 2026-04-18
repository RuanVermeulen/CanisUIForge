namespace CanisUIForge.IntegrationTests;

public class ContractsResolutionTests : IDisposable
{
    private readonly TestServiceFactory _factory;
    private readonly IContractsResolver _resolver;

    public ContractsResolutionTests()
    {
        _factory = new TestServiceFactory();
        _resolver = _factory.CreateContractsResolver();
    }

    [Fact]
    public async Task ResolveAsync_WithProjectReference_LoadsTypeRegistry()
    {
        string contractsPath = TestPaths.GetContractsProjectPath();
        ContractsConfig config = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = contractsPath
        };

        ITypeRegistry registry = await _resolver.ResolveAsync(config);

        Assert.NotNull(registry);
        Assert.True(registry.GetAll().Count > 0, "Registry should contain at least one type.");
    }

    [Fact]
    public async Task ResolveAsync_WithProjectReference_ContainsRequestTypes()
    {
        string contractsPath = TestPaths.GetContractsProjectPath();
        ContractsConfig config = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = contractsPath
        };

        ITypeRegistry registry = await _resolver.ResolveAsync(config);

        Assert.True(registry.Contains("CreateCustomerRequest"), "Registry should contain CreateCustomerRequest.");
        Assert.True(registry.Contains("UpdateCustomerRequest"), "Registry should contain UpdateCustomerRequest.");
        Assert.True(registry.Contains("CreateProductRequest"), "Registry should contain CreateProductRequest.");
        Assert.True(registry.Contains("UpdateProductRequest"), "Registry should contain UpdateProductRequest.");
        Assert.True(registry.Contains("CreateOrderRequest"), "Registry should contain CreateOrderRequest.");
        Assert.True(registry.Contains("UpdateOrderRequest"), "Registry should contain UpdateOrderRequest.");
    }

    [Fact]
    public async Task ResolveAsync_WithProjectReference_ContainsResponseTypes()
    {
        string contractsPath = TestPaths.GetContractsProjectPath();
        ContractsConfig config = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = contractsPath
        };

        ITypeRegistry registry = await _resolver.ResolveAsync(config);

        Assert.True(registry.Contains("CustomerResponse"), "Registry should contain CustomerResponse.");
        Assert.True(registry.Contains("ProductResponse"), "Registry should contain ProductResponse.");
        Assert.True(registry.Contains("OrderResponse"), "Registry should contain OrderResponse.");
        Assert.True(registry.Contains("SearchCustomersResponse"), "Registry should contain SearchCustomersResponse.");
    }

    [Fact]
    public async Task ResolveAsync_WithProjectReference_TypesHaveCorrectProperties()
    {
        string contractsPath = TestPaths.GetContractsProjectPath();
        ContractsConfig config = new ContractsConfig
        {
            Mode = ContractsMode.ProjectReference,
            ProjectPath = contractsPath
        };

        ITypeRegistry registry = await _resolver.ResolveAsync(config);

        Type? customerResponseType = registry.Resolve("CustomerResponse");
        Assert.NotNull(customerResponseType);

        List<string> propertyNames = customerResponseType.GetProperties()
            .Select(p => p.Name)
            .ToList();

        Assert.Contains("Id", propertyNames);
        Assert.Contains("FirstName", propertyNames);
        Assert.Contains("LastName", propertyNames);
        Assert.Contains("Email", propertyNames);
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}
