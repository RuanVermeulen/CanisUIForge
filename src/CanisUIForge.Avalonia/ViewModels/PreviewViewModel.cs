namespace CanisUIForge.Avalonia.ViewModels;

public class PreviewViewModel : ViewModelBase
{
    private readonly IConfigValidator _configValidator;
    private readonly IOpenApiScanner _scanner;
    private readonly IContractsResolver _contractsResolver;
    private readonly IGenerationPlanBuilder _planBuilder;

    public PreviewViewModel(
        IConfigValidator configValidator,
        IOpenApiScanner scanner,
        IContractsResolver contractsResolver,
        IGenerationPlanBuilder planBuilder)
    {
        _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _contractsResolver = contractsResolver ?? throw new ArgumentNullException(nameof(contractsResolver));
        _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
    }

    public GenerationPlan? Plan { get; private set; }

    public bool IsLoading { get; private set; }

    public bool HasPlan => Plan is not null;

    public string SolutionName => Plan?.SolutionName ?? string.Empty;

    public string NamespaceRoot => Plan?.NamespaceRoot ?? string.Empty;

    public string OutputPath => Plan?.OutputPath ?? string.Empty;

    public string ApiTitle => Plan is not null ? $"{Plan.ApiTitle} v{Plan.ApiVersion}" : string.Empty;

    public List<TargetPlatform> Targets => Plan?.Targets ?? new List<TargetPlatform>();

    public List<ResolvedResource> Resources => Plan?.Resources ?? new List<ResolvedResource>();

    public TestConfig Tests => Plan?.Tests ?? new TestConfig();

    public event Action? PlanLoaded;

    public void SyncFromState(WizardState state)
    {
        Plan = null;
    }

    public async Task BuildPlanAsync(WizardState state)
    {
        ClearErrors();
        IsLoading = true;

        try
        {
            ForgeConfig config = state.ToForgeConfig();

            ConfigValidationResult validation = _configValidator.Validate(config);

            if (!validation.IsValid)
            {
                foreach (string error in validation.Errors)
                {
                    AddError(error);
                }

                return;
            }

            ApiDefinition apiDefinition = await _scanner.ScanAsync(config.SwaggerSource);
            ITypeRegistry typeRegistry = await _contractsResolver.ResolveAsync(config.Contracts);
            Plan = _planBuilder.Build(config, apiDefinition, typeRegistry);
            PlanLoaded?.Invoke();
        }
        catch (Exception exception)
        {
            AddError($"Failed to build plan: {exception.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
