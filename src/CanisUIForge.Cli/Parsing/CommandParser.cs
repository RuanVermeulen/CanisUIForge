namespace CanisUIForge.Cli.Parsing;

public class CommandParser
{
    public CliOptions Parse(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            throw new ArgumentException("No command specified.");
        }

        CliOptions options = new CliOptions();

        string commandText = arguments[0].ToLowerInvariant();
        options.Command = commandText switch
        {
            "generate" => CliCommand.Generate,
            "scan" => CliCommand.Scan,
            "preview" => CliCommand.Preview,
            _ => throw new ArgumentException($"Unknown command: {arguments[0]}")
        };

        int index = 1;
        while (index < arguments.Length)
        {
            string argument = arguments[index];
            switch (argument.ToLowerInvariant())
            {
                case "--config":
                    options.ConfigFilePath = GetNextValue(arguments, ref index);
                    break;
                case "--name":
                    options.SolutionName = GetNextValue(arguments, ref index);
                    break;
                case "--swagger":
                    options.SwaggerSource = GetNextValue(arguments, ref index);
                    break;
                case "--output":
                    options.OutputPath = GetNextValue(arguments, ref index);
                    break;
                case "--namespace":
                    options.NamespaceRoot = GetNextValue(arguments, ref index);
                    break;
                case "--contracts-mode":
                    options.ContractsMode = GetNextValue(arguments, ref index);
                    break;
                case "--contracts-project":
                    options.ContractsProjectPath = GetNextValue(arguments, ref index);
                    break;
                case "--contracts-package":
                    options.ContractsPackageId = GetNextValue(arguments, ref index);
                    break;
                case "--contracts-version":
                    options.ContractsPackageVersion = GetNextValue(arguments, ref index);
                    break;
                case "--contracts-feed":
                    options.ContractsLocalFeed = GetNextValue(arguments, ref index);
                    break;
                case "--targets":
                    options.Targets = GetNextValue(arguments, ref index);
                    break;
                case "--unit-tests":
                    options.UnitTests = true;
                    break;
                case "--playwright-tests":
                    options.PlaywrightTests = true;
                    break;
                case "--appium-tests":
                    options.AppiumTests = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {argument}");
            }

            index++;
        }

        return options;
    }

    private static string GetNextValue(string[] arguments, ref int index)
    {
        index++;

        if (index >= arguments.Length)
        {
            throw new ArgumentException($"Missing value for {arguments[index - 1]}");
        }

        return arguments[index];
    }
}
