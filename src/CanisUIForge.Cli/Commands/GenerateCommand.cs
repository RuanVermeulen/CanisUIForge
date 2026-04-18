namespace CanisUIForge.Cli.Commands;

public class GenerateCommand : ICommand
{
    private readonly ConfigResolver _configResolver;
    private readonly IConfigValidator _configValidator;
    private readonly IOpenApiScanner _scanner;
    private readonly IContractsResolver _contractsResolver;
    private readonly IGenerationPlanBuilder _planBuilder;
    private readonly GenerationExecutor _executor;
    private readonly IRegenerationTracker _tracker;
    private readonly IForgeLogger _logger;
    private readonly IPipelineValidator _pipelineValidator;

    public GenerateCommand(
        ConfigResolver configResolver,
        IConfigValidator configValidator,
        IOpenApiScanner scanner,
        IContractsResolver contractsResolver,
        IGenerationPlanBuilder planBuilder,
        GenerationExecutor executor,
        IRegenerationTracker tracker,
        IForgeLogger logger,
        IPipelineValidator pipelineValidator)
    {
        _configResolver = configResolver ?? throw new ArgumentNullException(nameof(configResolver));
        _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _contractsResolver = contractsResolver ?? throw new ArgumentNullException(nameof(contractsResolver));
        _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pipelineValidator = pipelineValidator ?? throw new ArgumentNullException(nameof(pipelineValidator));
    }

    public async Task<int> ExecuteAsync(CliOptions options)
    {
        _logger.Log(ForgeLogLevel.Information, "Loading configuration...", "Config");
        ForgeConfig config = await _configResolver.ResolveAsync(options);

        _logger.Log(ForgeLogLevel.Information, "Validating configuration...", "Config");
        ConfigValidationResult configValidation = _configValidator.Validate(config);

        if (!configValidation.IsValid)
        {
            foreach (string error in configValidation.Errors)
            {
                _logger.Log(ForgeLogLevel.Error, error, "Config");
            }

            return 1;
        }

        _logger.Log(ForgeLogLevel.Information, "Validating contracts source...", "Contracts");
        PipelineValidationResult contractsValidation = _pipelineValidator.ValidateContractsSource(config.Contracts);

        foreach (string warning in contractsValidation.Warnings)
        {
            _logger.Log(ForgeLogLevel.Warning, warning, "Contracts");
        }

        if (!contractsValidation.IsValid)
        {
            foreach (string error in contractsValidation.Errors)
            {
                _logger.Log(ForgeLogLevel.Error, error, "Contracts");
            }

            return 1;
        }

        _logger.Log(ForgeLogLevel.Information, "Scanning Swagger...", "OpenApi");
        ApiDefinition apiDefinition = await _scanner.ScanAsync(config.SwaggerSource);
        _logger.Log(ForgeLogLevel.Information, $"Found {apiDefinition.Resources.Count} resource(s).", "OpenApi");

        _logger.Log(ForgeLogLevel.Information, "Resolving contracts...", "Contracts");
        ITypeRegistry typeRegistry = await _contractsResolver.ResolveAsync(config.Contracts);

        _logger.Log(ForgeLogLevel.Information, "Validating type mappings...", "Contracts");
        PipelineValidationResult schemaValidation = _pipelineValidator.ValidateSchemaResolution(apiDefinition, typeRegistry);

        foreach (string warning in schemaValidation.Warnings)
        {
            _logger.Log(ForgeLogLevel.Warning, warning, "Contracts");
        }

        if (!schemaValidation.IsValid)
        {
            foreach (string error in schemaValidation.Errors)
            {
                _logger.Log(ForgeLogLevel.Error, error, "Contracts");
            }

            return 1;
        }

        _logger.Log(ForgeLogLevel.Information, "Building generation plan...", "Planning");
        GenerationPlan plan = _planBuilder.Build(config, apiDefinition, typeRegistry);

        _logger.Log(ForgeLogLevel.Information, "Executing generation...", "Generation");
        await _executor.ExecuteAsync(plan);

        RegenerationResult result = _tracker.GetResult();
        _logger.Log(ForgeLogLevel.Information, $"Files created: {result.CreatedCount}, overwritten: {result.OverwrittenCount}, skipped: {result.SkippedCount}", "Summary");

        if (result.SkippedCount > 0)
        {
            foreach (RegenerationEntry entry in result.GetEntriesByAction(RegenerationAction.Skipped))
            {
                _logger.Log(ForgeLogLevel.Warning, $"Skipped (manual): {entry.FilePath}", "Summary");
            }
        }

        _logger.Log(ForgeLogLevel.Success, "Generation completed successfully!");

        return 0;
    }
}
