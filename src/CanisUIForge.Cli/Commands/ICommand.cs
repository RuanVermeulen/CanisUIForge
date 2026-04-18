namespace CanisUIForge.Cli.Commands;

public interface ICommand
{
    Task<int> ExecuteAsync(CliOptions options);
}
