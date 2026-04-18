namespace CanisUIForge.Generation.Logging;

public class ForgeLogEntry
{
    public DateTime Timestamp { get; set; }

    public ForgeLogLevel Level { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Category { get; set; }
}
