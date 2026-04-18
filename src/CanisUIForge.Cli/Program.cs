namespace CanisUIForge.Cli;

public static class Program
{
    public static async Task<int> Main(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        try
        {
            CommandParser parser = new CommandParser();
            CliOptions options = parser.Parse(arguments);

            ServiceFactory factory = new ServiceFactory();
            ICommand command = factory.CreateCommand(options.Command);

            return await command.ExecuteAsync(options);
        }
        catch (Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Error: {exception.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("CanisUIForge CLI");
        Console.WriteLine();
        Console.WriteLine("Usage: canis-uiforge <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  generate    Generate UI application from Swagger and Contracts");
        Console.WriteLine("  scan        Scan Swagger and display discovered resources");
        Console.WriteLine("  preview     Preview generation plan without generating files");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --config <path>              Path to JSON configuration file");
        Console.WriteLine("  --name <name>                Solution name");
        Console.WriteLine("  --swagger <path>             Path or URL to Swagger JSON");
        Console.WriteLine("  --contracts-mode <mode>      Contracts mode: project or nuget");
        Console.WriteLine("  --contracts-project <path>   Contracts project path");
        Console.WriteLine("  --contracts-package <id>     NuGet package ID");
        Console.WriteLine("  --contracts-version <ver>    NuGet package version");
        Console.WriteLine("  --contracts-feed <feed>      NuGet local feed path");
        Console.WriteLine("  --targets <t1,t2>            Target platforms: blazor, maui, both");
        Console.WriteLine("  --output <path>              Output directory");
        Console.WriteLine("  --namespace <ns>             Root namespace");
        Console.WriteLine("  --unit-tests                 Enable unit test generation");
        Console.WriteLine("  --playwright-tests           Enable Playwright test generation");
        Console.WriteLine("  --appium-tests               Enable Appium test generation");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  canis-uiforge generate --config forge.json");
        Console.WriteLine("  canis-uiforge generate --name AdminPortal --swagger swagger.json \\");
        Console.WriteLine("    --contracts-mode project --contracts-project ../Contracts.csproj \\");
        Console.WriteLine("    --targets blazor,maui");
        Console.WriteLine("  canis-uiforge scan --swagger swagger.json");
        Console.WriteLine("  canis-uiforge preview --config forge.json");
    }
}
