namespace CanisUIForge.Generation.Output;

public class RegenerationResult
{
    private readonly List<RegenerationEntry> _entries = new List<RegenerationEntry>();

    public IReadOnlyList<RegenerationEntry> Entries => _entries;

    public int CreatedCount => _entries.Count(entry => entry.Action == RegenerationAction.Created);

    public int OverwrittenCount => _entries.Count(entry => entry.Action == RegenerationAction.Overwritten);

    public int SkippedCount => _entries.Count(entry => entry.Action == RegenerationAction.Skipped);

    public int TotalCount => _entries.Count;

    public void Add(RegenerationEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));

        _entries.Add(entry);
    }

    public IReadOnlyList<RegenerationEntry> GetEntriesByAction(RegenerationAction action)
    {
        return _entries.Where(entry => entry.Action == action).ToList();
    }

    public void Clear()
    {
        _entries.Clear();
    }
}
