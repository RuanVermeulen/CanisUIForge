namespace CanisUIForge.Cli.Commands;

public class ScanCommand : ICommand
{
    private readonly IOpenApiScanner _scanner;

    public ScanCommand(IOpenApiScanner scanner)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public async Task<int> ExecuteAsync(CliOptions options)
    {
        string swaggerSource = options.SwaggerSource;

        if (string.IsNullOrWhiteSpace(swaggerSource) && !string.IsNullOrWhiteSpace(options.ConfigFilePath))
        {
            JsonConfigLoader loader = new JsonConfigLoader();
            ForgeConfig config = await loader.LoadAsync(options.ConfigFilePath);
            swaggerSource = config.SwaggerSource;
        }

        if (string.IsNullOrWhiteSpace(swaggerSource))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Swagger source is required. Use --swagger <path> or --config <path>.");
            Console.ResetColor();
            return 1;
        }

        Console.WriteLine("Scanning Swagger...");
        ApiDefinition apiDefinition = await _scanner.ScanAsync(swaggerSource);

        Console.WriteLine();
        Console.WriteLine($"API: {apiDefinition.Title} v{apiDefinition.Version}");

        if (!string.IsNullOrWhiteSpace(apiDefinition.Description))
        {
            Console.WriteLine($"Description: {apiDefinition.Description}");
        }

        Console.WriteLine($"Resources found: {apiDefinition.Resources.Count}");
        Console.WriteLine();

        foreach (ResourceDefinition resource in apiDefinition.Resources)
        {
            Console.WriteLine($"  {resource.Name}");

            foreach (EndpointDefinition endpoint in resource.Endpoints)
            {
                Console.WriteLine($"    {endpoint.Method,-8} {endpoint.Route,-40} [{endpoint.Classification}]");
            }

            Console.WriteLine();
        }

        return 0;
    }
}
