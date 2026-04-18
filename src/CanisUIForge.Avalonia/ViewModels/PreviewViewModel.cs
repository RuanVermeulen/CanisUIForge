namespace CanisUIForge.Avalonia.ViewModels;

public class PreviewViewModel : ViewModelBase
{
    private readonly IConfigValidator _configValidator;
    private readonly IOpenApiScanner _scanner;
    private readonly IContractsResolver _contractsResolver;
    private readonly IGenerationPlanBuilder _planBuilder;
    private readonly IPipelineValidator _pipelineValidator;

    public PreviewViewModel(
        IConfigValidator configValidator,
        IOpenApiScanner scanner,
        IContractsResolver contractsResolver,
        IGenerationPlanBuilder planBuilder,
        IPipelineValidator pipelineValidator)
    {
        _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _contractsResolver = contractsResolver ?? throw new ArgumentNullException(nameof(contractsResolver));
        _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        _pipelineValidator = pipelineValidator ?? throw new ArgumentNullException(nameof(pipelineValidator));
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

    public List<string> ValidationWarnings { get; } = new List<string>();

    public TestConfig Tests => Plan?.Tests ?? new TestConfig();

    public event Action? PlanLoaded;

    public void SyncFromState(WizardState state)
    {
        Plan = null;
        ValidationWarnings.Clear();
    }

    public async Task BuildPlanAsync(WizardState state)
    {
        ClearErrors();
        ValidationWarnings.Clear();
        IsLoading = true;

        try
        {
            ForgeConfig config = state.ToForgeConfig();

            ConfigValidationResult configValidation = _configValidator.Validate(config);

            if (!configValidation.IsValid)
            {
                foreach (string error in configValidation.Errors)
                {
                    AddError(error);
                }

                return;
            }

            PipelineValidationResult contractsValidation = _pipelineValidator.ValidateContractsSource(config.Contracts);

            foreach (string warning in contractsValidation.Warnings)
            {
                ValidationWarnings.Add(warning);
            }

            if (!contractsValidation.IsValid)
            {
                foreach (string error in contractsValidation.Errors)
                {
                    AddError(error);
                }

                return;
            }

            ApiDefinition apiDefinition = await _scanner.ScanAsync(config.SwaggerSource);
            ITypeRegistry typeRegistry = await _contractsResolver.ResolveAsync(config.Contracts);

            PipelineValidationResult schemaValidation = _pipelineValidator.ValidateSchemaResolution(apiDefinition, typeRegistry);

            foreach (string warning in schemaValidation.Warnings)
            {
                ValidationWarnings.Add(warning);
            }

            if (!schemaValidation.IsValid)
            {
                foreach (string error in schemaValidation.Errors)
                {
                    AddError(error);
                }

                return;
            }

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
