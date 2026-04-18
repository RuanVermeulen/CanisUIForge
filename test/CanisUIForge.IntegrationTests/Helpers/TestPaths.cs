namespace CanisUIForge.IntegrationTests.Helpers;

public static class TestPaths
{
    public static string GetTestDataDirectory()
    {
        string assemblyLocation = Path.GetDirectoryName(typeof(TestPaths).Assembly.Location)!;
        return Path.Combine(assemblyLocation, "TestData");
    }

    public static string GetSwaggerPath()
    {
        return Path.Combine(GetTestDataDirectory(), "swagger.json");
    }

    public static string GetForgeConfigPath()
    {
        return Path.Combine(GetTestDataDirectory(), "forge.json");
    }

    public static string GetContractsProjectPath()
    {
        string assemblyLocation = Path.GetDirectoryName(typeof(TestPaths).Assembly.Location)!;
        return Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", "..", "TestApi", "TestApi.Contracts", "TestApi.Contracts.csproj"));
    }

    public static string CreateTempOutputDirectory()
    {
        string tempPath = Path.Combine(Path.GetTempPath(), "CanisUIForge_IntegrationTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    public static void CleanupTempDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (IOException)
            {
                // Best effort cleanup
            }
        }
    }
}
