namespace CanisUIForge.Generation.Output;

public class FileWriter : IFileWriter
{
    private readonly IRegenerationTracker? _tracker;

    public FileWriter()
    {
    }

    public FileWriter(IRegenerationTracker tracker)
    {
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    }

    public async Task WriteGeneratedFileAsync(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        string markedContent = GeneratedFileHeader.AddHeader(filePath, content);

        if (File.Exists(filePath))
        {
            if (GeneratedFileHeader.SupportsHeader(filePath) && !GeneratedFileHeader.IsGeneratedFile(filePath))
            {
                _tracker?.Record(filePath, RegenerationAction.Skipped);
                return;
            }

            await File.WriteAllTextAsync(filePath, markedContent);
            _tracker?.Record(filePath, RegenerationAction.Overwritten);
        }
        else
        {
            EnsureDirectoryForFile(filePath);
            await File.WriteAllTextAsync(filePath, markedContent);
            _tracker?.Record(filePath, RegenerationAction.Created);
        }
    }

    public async Task WriteFileIfNotExistsAsync(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        if (File.Exists(filePath))
        {
            _tracker?.Record(filePath, RegenerationAction.Skipped);
            return;
        }

        EnsureDirectoryForFile(filePath);
        await File.WriteAllTextAsync(filePath, content);
        _tracker?.Record(filePath, RegenerationAction.Created);
    }

    public void EnsureDirectoryExists(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Directory path must not be null or empty.", nameof(directoryPath));
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static void EnsureDirectoryForFile(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
