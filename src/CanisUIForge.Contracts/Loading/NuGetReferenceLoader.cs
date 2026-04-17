using System.Reflection;
using System.Runtime.Loader;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace CanisUIForge.Contracts.Loading;

public class NuGetReferenceLoader : IAssemblyLoader
{
    private readonly string _packageId;
    private readonly string _packageVersion;
    private readonly string _localFeed;

    public NuGetReferenceLoader(string packageId, string packageVersion, string localFeed)
    {
        if (string.IsNullOrWhiteSpace(packageId))
        {
            throw new ArgumentException("Package ID must not be null or empty.", nameof(packageId));
        }

        if (string.IsNullOrWhiteSpace(packageVersion))
        {
            throw new ArgumentException("Package version must not be null or empty.", nameof(packageVersion));
        }

        _packageId = packageId;
        _packageVersion = packageVersion;
        _localFeed = localFeed ?? string.Empty;
    }

    public async Task<Assembly> LoadAsync()
    {
        string packageDirectory = Path.Combine(Path.GetTempPath(), "CanisUIForge", "packages");
        Directory.CreateDirectory(packageDirectory);

        string assemblyPath = await DownloadAndExtractPackageAsync(packageDirectory);

        if (!File.Exists(assemblyPath))
        {
            throw new FileNotFoundException(
                $"Assembly not found in NuGet package '{_packageId}' version '{_packageVersion}': {assemblyPath}",
                assemblyPath);
        }

        AssemblyLoadContext loadContext = new AssemblyLoadContext("NuGetContractsLoader", isCollectible: true);
        using FileStream assemblyStream = File.OpenRead(assemblyPath);
        Assembly assembly = loadContext.LoadFromStream(assemblyStream);

        return assembly;
    }

    private async Task<string> DownloadAndExtractPackageAsync(string packageDirectory)
    {
        SourceRepository repository = CreateSourceRepository();
        FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();
        NuGetVersion version = new NuGetVersion(_packageVersion);

        string packageFilePath = Path.Combine(packageDirectory, $"{_packageId}.{_packageVersion}.nupkg");

        using (FileStream packageStream = File.Create(packageFilePath))
        {
            bool downloaded = await resource.CopyNupkgToStreamAsync(
                _packageId,
                version,
                packageStream,
                new SourceCacheContext(),
                NullLogger.Instance,
                CancellationToken.None);

            if (!downloaded)
            {
                throw new InvalidOperationException(
                    $"Failed to download NuGet package '{_packageId}' version '{_packageVersion}'.");
            }
        }

        string extractDirectory = Path.Combine(packageDirectory, $"{_packageId}.{_packageVersion}");

        using (PackageArchiveReader packageReader = new PackageArchiveReader(packageFilePath))
        {
            IEnumerable<FrameworkSpecificGroup> libItems = packageReader.GetLibItems().ToList();

            FrameworkSpecificGroup? targetGroup = libItems
                .FirstOrDefault(group => group.TargetFramework.GetShortFolderName().StartsWith("net8", StringComparison.OrdinalIgnoreCase))
                ?? libItems.FirstOrDefault(group => group.TargetFramework.GetShortFolderName().StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
                ?? libItems.FirstOrDefault();

            if (targetGroup is null)
            {
                throw new InvalidOperationException(
                    $"No compatible framework found in NuGet package '{_packageId}' version '{_packageVersion}'.");
            }

            string? dllItem = targetGroup.Items
                .FirstOrDefault(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

            if (dllItem is null)
            {
                throw new InvalidOperationException(
                    $"No DLL found in NuGet package '{_packageId}' version '{_packageVersion}'.");
            }

            Directory.CreateDirectory(extractDirectory);

            string extractedDllPath = Path.Combine(extractDirectory, Path.GetFileName(dllItem));
            using Stream entryStream = packageReader.GetStream(dllItem);
            using FileStream outputStream = File.Create(extractedDllPath);
            await entryStream.CopyToAsync(outputStream);

            return extractedDllPath;
        }
    }

    private SourceRepository CreateSourceRepository()
    {
        if (!string.IsNullOrWhiteSpace(_localFeed))
        {
            return Repository.Factory.GetCoreV3(_localFeed);
        }

        return Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
    }
}
