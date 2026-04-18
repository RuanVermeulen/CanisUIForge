namespace CanisUIForge.Cli.Commands;

public class ConfigResolver
{
    private readonly IConfigLoader _configLoader;

    public ConfigResolver(IConfigLoader configLoader)
    {
        _configLoader = configLoader ?? throw new ArgumentNullException(nameof(configLoader));
    }

    public async Task<ForgeConfig> ResolveAsync(CliOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ConfigFilePath))
        {
            return await _configLoader.LoadAsync(options.ConfigFilePath);
        }

        return BuildFromOptions(options);
    }

    private static ForgeConfig BuildFromOptions(CliOptions options)
    {
        ForgeConfig config = new ForgeConfig
        {
            SolutionName = options.SolutionName,
            SwaggerSource = options.SwaggerSource,
            OutputPath = !string.IsNullOrWhiteSpace(options.OutputPath)
                ? options.OutputPath
                : Directory.GetCurrentDirectory(),
            NamespaceRoot = options.NamespaceRoot
        };

        if (!string.IsNullOrWhiteSpace(options.Targets))
        {
            string[] targetParts = options.Targets.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (string target in targetParts)
            {
                if (Enum.TryParse(target.Trim(), ignoreCase: true, out TargetPlatform platform))
                {
                    config.Targets.Add(platform);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(options.ContractsMode))
        {
            string modeText = options.ContractsMode.ToLowerInvariant();

            config.Contracts = new ContractsConfig
            {
                Mode = modeText switch
                {
                    "project" => Core.Enums.ContractsMode.ProjectReference,
                    "nuget" => Core.Enums.ContractsMode.NuGetReference,
                    _ => throw new ArgumentException($"Unknown contracts mode: {options.ContractsMode}")
                },
                ProjectPath = options.ContractsProjectPath,
                PackageId = options.ContractsPackageId,
                PackageVersion = options.ContractsPackageVersion,
                LocalFeed = options.ContractsLocalFeed
            };
        }

        config.Tests = new TestConfig
        {
            Unit = options.UnitTests,
            Playwright = options.PlaywrightTests,
            Appium = options.AppiumTests
        };

        return config;
    }
}
