namespace CanisUIForge.Generation.Output;

public class RegenerationTracker : IRegenerationTracker
{
    private readonly RegenerationResult _result = new RegenerationResult();

    public void Record(string filePath, RegenerationAction action)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));

        RegenerationEntry entry = new RegenerationEntry(filePath, action);
        _result.Add(entry);
    }

    public RegenerationResult GetResult()
    {
        return _result;
    }

    public void Reset()
    {
        _result.Clear();
    }
}
