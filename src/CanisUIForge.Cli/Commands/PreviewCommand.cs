namespace CanisUIForge.Cli.Commands;

public class PreviewCommand : ICommand
{
    private readonly ConfigResolver _configResolver;
    private readonly IConfigValidator _configValidator;
    private readonly IOpenApiScanner _scanner;
    private readonly IContractsResolver _contractsResolver;
    private readonly IGenerationPlanBuilder _planBuilder;

    public PreviewCommand(
        ConfigResolver configResolver,
        IConfigValidator configValidator,
        IOpenApiScanner scanner,
        IContractsResolver contractsResolver,
        IGenerationPlanBuilder planBuilder)
    {
        _configResolver = configResolver ?? throw new ArgumentNullException(nameof(configResolver));
        _configValidator = configValidator ?? throw new ArgumentNullException(nameof(configValidator));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _contractsResolver = contractsResolver ?? throw new ArgumentNullException(nameof(contractsResolver));
        _planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
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

        Console.WriteLine("Resolving contracts...");
        ITypeRegistry typeRegistry = await _contractsResolver.ResolveAsync(config.Contracts);

        Console.WriteLine("Building generation plan...");
        GenerationPlan plan = _planBuilder.Build(config, apiDefinition, typeRegistry);

        Console.WriteLine();
        PrintPlan(plan);

        return 0;
    }

    private static void PrintPlan(GenerationPlan plan)
    {
        Console.WriteLine("=== Generation Plan Preview ===");
        Console.WriteLine();
        Console.WriteLine($"  Solution:  {plan.SolutionName}");
        Console.WriteLine($"  Namespace: {plan.NamespaceRoot}");
        Console.WriteLine($"  Output:    {plan.OutputPath}");
        Console.WriteLine($"  API:       {plan.ApiTitle} v{plan.ApiVersion}");
        Console.WriteLine();

        Console.WriteLine("  Targets:");

        foreach (TargetPlatform target in plan.Targets)
        {
            Console.WriteLine($"    - {target}");
        }

        Console.WriteLine();
        Console.WriteLine("  Resources:");

        foreach (ResolvedResource resource in plan.Resources)
        {
            Console.WriteLine($"    {resource.Name} ({resource.Style})");

            foreach (ResolvedEndpoint endpoint in resource.Endpoints)
            {
                Console.WriteLine($"      {endpoint.Method,-8} {endpoint.Route,-40} [{endpoint.Classification}]");
            }
        }

        Console.WriteLine();
        Console.WriteLine("  Tests:");
        Console.WriteLine($"    Unit:       {(plan.Tests.Unit ? "Yes" : "No")}");
        Console.WriteLine($"    Playwright: {(plan.Tests.Playwright ? "Yes" : "No")}");
        Console.WriteLine($"    Appium:     {(plan.Tests.Appium ? "Yes" : "No")}");
        Console.WriteLine();
    }
}
