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

    public GenerateCommand(
        ConfigResolver configResolver,
        IConfigValidator configValidator,
        IOpenApiScanner scanner,
        IContractsResolver contractsResolver,
        IGenerationPlanBuilder planBuilder,
        GenerationExecutor executor,
        IRegenerationTracker tracker)
    {
        _configResolver = configResolver ?? throw new ArgumentNullException(nameof(configResolver));
        _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _contractsResolver = contractsResolver ?? throw new ArgumentNullException(nameof(contractsResolver));
        _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    }

    public async Task<int> ExecuteAsync(CliOptions options)
    {
        Console.WriteLine("Loading configuration...");
        ForgeConfig config = await _configResolver.ResolveAsync(options);

        Console.WriteLine("Validating configuration...");
        ConfigValidationResult validation = _configValidator.Validate(config);

        if (!validation.IsValid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Configuration validation failed:");

            foreach (string error in validation.Errors)
            {
                Console.Error.WriteLine($"  - {error}");
            }

            Console.ResetColor();
            return 1;
        }

        Console.WriteLine("Scanning Swagger...");
        ApiDefinition apiDefinition = await _scanner.ScanAsync(config.SwaggerSource);
        Console.WriteLine($"  Found {apiDefinition.Resources.Count} resource(s).");

        Console.WriteLine("Resolving contracts...");
        ITypeRegistry typeRegistry = await _contractsResolver.ResolveAsync(config.Contracts);

        Console.WriteLine("Building generation plan...");
        GenerationPlan plan = _planBuilder.Build(config, apiDefinition, typeRegistry);

        Console.WriteLine("Executing generation...");
        await _executor.ExecuteAsync(plan);

        RegenerationResult result = _tracker.GetResult();
        Console.WriteLine();
        Console.WriteLine($"  Files created:     {result.CreatedCount}");
        Console.WriteLine($"  Files overwritten: {result.OverwrittenCount}");
        Console.WriteLine($"  Files skipped:     {result.SkippedCount}");

        if (result.SkippedCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  Skipped files (manual modifications preserved):");

            foreach (RegenerationEntry entry in result.GetEntriesByAction(RegenerationAction.Skipped))
            {
                Console.WriteLine($"    - {entry.FilePath}");
            }

            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Generation completed successfully!");
        Console.ResetColor();

        return 0;
    }
}
