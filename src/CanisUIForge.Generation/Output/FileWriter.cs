namespace CanisUIForge.Generation.Output;

public class FileWriter : IFileWriter
{
    public async Task WriteGeneratedFileAsync(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        EnsureDirectoryForFile(filePath);
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task WriteFileIfNotExistsAsync(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        if (File.Exists(filePath))
        {
            return;
        }

        EnsureDirectoryForFile(filePath);
        await File.WriteAllTextAsync(filePath, content);
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
