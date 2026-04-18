namespace CanisUIForge.Generation.Logging;

public class ForgeLogger : IForgeLogger
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
        EntryLogged?.Invoke(entry);
    }

    public void Clear()
    {
        _entries.Clear();
    }
}
