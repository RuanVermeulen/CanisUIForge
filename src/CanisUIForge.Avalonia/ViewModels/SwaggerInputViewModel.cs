namespace CanisUIForge.Avalonia.ViewModels;

public class SwaggerInputViewModel : ViewModelBase
{
    private readonly IOpenApiScanner _scanner;

    public SwaggerInputViewModel(IOpenApiScanner scanner)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public string SwaggerSource { get; set; } = string.Empty;

    public ContractsMode ContractsMode { get; set; }

    public string ContractsProjectPath { get; set; } = string.Empty;

    public string ContractsPackageId { get; set; } = string.Empty;

    public string ContractsPackageVersion { get; set; } = string.Empty;

    public string ContractsLocalFeed { get; set; } = string.Empty;

    public ApiDefinition? ScannedApi { get; private set; }

    public bool IsScanning { get; private set; }

    public bool HasScanned => ScannedApi is not null;

    public event Action? ScanCompleted;

    public bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(SwaggerSource))
        {
            AddError("Swagger source is required.");
        }

        if (ContractsMode == ContractsMode.ProjectReference
            && string.IsNullOrWhiteSpace(ContractsProjectPath))
        {
            AddError("Contracts project path is required for ProjectReference mode.");
        }

        if (ContractsMode == ContractsMode.NuGetReference)
        {
            if (string.IsNullOrWhiteSpace(ContractsPackageId))
            {
                AddError("Package ID is required for NuGet mode.");
            }

            if (string.IsNullOrWhiteSpace(ContractsPackageVersion))
            {
                AddError("Package version is required for NuGet mode.");
            }
        }

        return !HasErrors;
    }

    public async Task ScanSwaggerAsync()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(SwaggerSource))
        {
            AddError("Swagger source is required.");
            return;
        }

        IsScanning = true;

        try
        {
            ScannedApi = await _scanner.ScanAsync(SwaggerSource);
            ScanCompleted?.Invoke();
        }
        catch (Exception exception)
        {
            AddError($"Swagger scan failed: {exception.Message}");
        }
        finally
        {
            IsScanning = false;
        }
    }

    public void SyncFromState(WizardState state)
    {
        SwaggerSource = state.SwaggerSource;
        ContractsMode = state.ContractsMode;
        ContractsProjectPath = state.ContractsProjectPath;
        ContractsPackageId = state.ContractsPackageId;
        ContractsPackageVersion = state.ContractsPackageVersion;
        ContractsLocalFeed = state.ContractsLocalFeed;
    }

    public void SyncToState(WizardState state)
    {
        state.SwaggerSource = SwaggerSource;
        state.ContractsMode = ContractsMode;
        state.ContractsProjectPath = ContractsProjectPath;
        state.ContractsPackageId = ContractsPackageId;
        state.ContractsPackageVersion = ContractsPackageVersion;
        state.ContractsLocalFeed = ContractsLocalFeed;
    }
}
