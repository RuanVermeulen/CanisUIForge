namespace CanisUIForge.Avalonia.ViewModels;

public class WizardViewModel : ViewModelBase
{
    private readonly WizardNavigator _navigator = new WizardNavigator();
    private readonly WizardState _state = new WizardState();

    private readonly ProjectSetupViewModel _projectSetup;
    private readonly SwaggerInputViewModel _swaggerInput;
    private readonly ControllerSelectionViewModel _controllerSelection;
    private readonly PreviewViewModel _preview;
    private readonly GenerationViewModel _generation;

    public WizardViewModel(
        ProjectSetupViewModel projectSetup,
        SwaggerInputViewModel swaggerInput,
        ControllerSelectionViewModel controllerSelection,
        PreviewViewModel preview,
        GenerationViewModel generation)
    {
        _projectSetup = projectSetup ?? throw new ArgumentNullException(nameof(projectSetup));
        _swaggerInput = swaggerInput ?? throw new ArgumentNullException(nameof(swaggerInput));
        _controllerSelection = controllerSelection ?? throw new ArgumentNullException(nameof(controllerSelection));
        _preview = preview ?? throw new ArgumentNullException(nameof(preview));
        _generation = generation ?? throw new ArgumentNullException(nameof(generation));
    }

    public WizardStep CurrentStep => _navigator.CurrentStep;

    public bool CanGoNext => _navigator.CanGoNext;

    public bool CanGoBack => _navigator.CanGoBack;

    public WizardState State => _state;

    public ProjectSetupViewModel ProjectSetup => _projectSetup;

    public SwaggerInputViewModel SwaggerInput => _swaggerInput;

    public ControllerSelectionViewModel ControllerSelection => _controllerSelection;

    public PreviewViewModel Preview => _preview;

    public GenerationViewModel Generation => _generation;

    public event Action? StepChanged;

    public void GoNext()
    {
        if (!CanGoNext)
        {
            return;
        }

        SyncStateFromCurrentStep();
        _navigator.GoNext();
        SyncStateToCurrentStep();
        StepChanged?.Invoke();
    }

    public void GoBack()
    {
        if (!CanGoBack)
        {
            return;
        }

        SyncStateFromCurrentStep();
        _navigator.GoBack();
        SyncStateToCurrentStep();
        StepChanged?.Invoke();
    }

    public async Task LoadConfigAsync(string filePath)
    {
        JsonConfigLoader loader = new JsonConfigLoader();
        ForgeConfig config = await loader.LoadAsync(filePath);

        _state.SolutionName = config.SolutionName;
        _state.OutputPath = config.OutputPath;
        _state.NamespaceRoot = config.NamespaceRoot;
        _state.Targets = new List<TargetPlatform>(config.Targets);
        _state.SwaggerSource = config.SwaggerSource;
        _state.ContractsMode = config.Contracts.Mode;
        _state.ContractsProjectPath = config.Contracts.ProjectPath;
        _state.ContractsPackageId = config.Contracts.PackageId;
        _state.ContractsPackageVersion = config.Contracts.PackageVersion;
        _state.ContractsLocalFeed = config.Contracts.LocalFeed;
        _state.EnableUnitTests = config.Tests.Unit;
        _state.EnablePlaywrightTests = config.Tests.Playwright;
        _state.EnableAppiumTests = config.Tests.Appium;

        SyncStateToCurrentStep();
    }

    public async Task SaveConfigAsync(string filePath)
    {
        SyncStateFromCurrentStep();
        ForgeConfig config = _state.ToForgeConfig();
        JsonConfigSaver saver = new JsonConfigSaver();
        await saver.SaveAsync(config, filePath);
    }

    private void SyncStateFromCurrentStep()
    {
        switch (_navigator.CurrentStep)
        {
            case WizardStep.ProjectSetup:
                _projectSetup.SyncToState(_state);
                break;
            case WizardStep.SwaggerInput:
                _swaggerInput.SyncToState(_state);
                break;
            case WizardStep.ControllerSelection:
                _controllerSelection.SyncToState(_state);
                break;
        }
    }

    private void SyncStateToCurrentStep()
    {
        switch (_navigator.CurrentStep)
        {
            case WizardStep.ProjectSetup:
                _projectSetup.SyncFromState(_state);
                break;
            case WizardStep.SwaggerInput:
                _swaggerInput.SyncFromState(_state);
                break;
            case WizardStep.ControllerSelection:
                _controllerSelection.SyncFromState(_state);
                break;
            case WizardStep.Preview:
                _preview.SyncFromState(_state);
                break;
        }
    }
}
