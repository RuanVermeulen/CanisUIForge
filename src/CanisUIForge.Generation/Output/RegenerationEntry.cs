namespace CanisUIForge.Generation.Output;

public class RegenerationEntry
{
    public RegenerationEntry(string filePath, RegenerationAction action)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        Action = action;
    }

    public string FilePath { get; }

    public RegenerationAction Action { get; }
}
