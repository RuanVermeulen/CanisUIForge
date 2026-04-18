namespace CanisUIForge.Generation.Logging;

public class ConsoleForgeLogger : IForgeLogger
{
    private readonly List<ForgeLogEntry> _entries = new List<ForgeLogEntry>();

    public IReadOnlyList<ForgeLogEntry> Entries => _entries;

    public event Action<ForgeLogEntry>? EntryLogged;

    public void Log(ForgeLogLevel level, string message, string? category = null)
    {
        ForgeLogEntry entry = new ForgeLogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message,
            Category = category
        };

        _entries.Add(entry);
        WriteToConsole(entry);
        EntryLogged?.Invoke(entry);
    }

    public void Clear()
    {
        _entries.Clear();
    }

    private static void WriteToConsole(ForgeLogEntry entry)
    {
        ConsoleColor previousColor = Console.ForegroundColor;

        switch (entry.Level)
        {
            case ForgeLogLevel.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case ForgeLogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case ForgeLogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        string prefix = entry.Level switch
        {
            ForgeLogLevel.Information => "INFO",
            ForgeLogLevel.Success => " OK ",
            ForgeLogLevel.Warning => "WARN",
            ForgeLogLevel.Error => "ERR ",
            _ => "    "
        };

        string categoryPrefix = !string.IsNullOrWhiteSpace(entry.Category)
            ? $"[{entry.Category}] "
            : string.Empty;

        Console.WriteLine($"  [{prefix}] {categoryPrefix}{entry.Message}");
        Console.ForegroundColor = previousColor;
    }
}
