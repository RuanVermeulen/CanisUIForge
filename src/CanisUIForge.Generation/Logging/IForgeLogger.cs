namespace CanisUIForge.Generation.Logging;

public interface IForgeLogger
{
    void Log(ForgeLogLevel level, string message, string? category = null);

    IReadOnlyList<ForgeLogEntry> Entries { get; }

    event Action<ForgeLogEntry>? EntryLogged;

    void Clear();
}
