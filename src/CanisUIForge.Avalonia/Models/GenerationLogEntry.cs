namespace CanisUIForge.Avalonia.Models;

public class GenerationLogEntry
{
    public DateTime Timestamp { get; set; }

    public string Message { get; set; } = string.Empty;

    public GenerationLogLevel Level { get; set; }
}
