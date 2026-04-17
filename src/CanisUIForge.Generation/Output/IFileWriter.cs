namespace CanisUIForge.Generation.Output;

public interface IFileWriter
{
    Task WriteGeneratedFileAsync(string filePath, string content);

    Task WriteFileIfNotExistsAsync(string filePath, string content);

    void EnsureDirectoryExists(string directoryPath);
}
