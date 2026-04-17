using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace CanisUIForge.Contracts.Loading;

public class ProjectReferenceLoader : IAssemblyLoader
{
    private readonly string _projectPath;

    public ProjectReferenceLoader(string projectPath)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentException("Project path must not be null or empty.", nameof(projectPath));
        }

        _projectPath = projectPath;
    }

    public async Task<Assembly> LoadAsync()
    {
        string fullProjectPath = Path.GetFullPath(_projectPath);

        if (!File.Exists(fullProjectPath))
        {
            throw new FileNotFoundException($"Project file not found: {fullProjectPath}", fullProjectPath);
        }

        await BuildProjectAsync(fullProjectPath);

        string assemblyPath = ResolveOutputAssemblyPath(fullProjectPath);

        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException($"Built assembly not found: {assemblyPath}", assemblyPath);
        }

        AssemblyLoadContext loadContext = new AssemblyLoadContext("ContractsLoader", isCollectible: true);
        using FileStream assemblyStream = File.OpenRead(assemblyPath);
        Assembly assembly = loadContext.LoadFromStream(assemblyStream);

        return assembly;
    }

    private static async Task BuildProjectAsync(string projectPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\" --configuration Release --no-restore",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? process = Process.Start(startInfo);

        if (process is null)
        {
            throw new InvalidOperationException("Failed to start dotnet build process.");
        }

        string standardOutput = await process.StandardOutput.ReadToEndAsync();
        string standardError = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"dotnet build failed for project '{projectPath}'. Exit code: {process.ExitCode}. " +
                $"Output: {standardOutput}. Error: {standardError}");
        }
    }

    private static string ResolveOutputAssemblyPath(string projectPath)
    {
        string projectDirectory = Path.GetDirectoryName(projectPath) ?? string.Empty;
        string projectName = Path.GetFileNameWithoutExtension(projectPath);
        string assemblyPath = Path.Combine(projectDirectory, "bin", "Release", "net8.0", $"{projectName}.dll");

        return assemblyPath;
    }
}
