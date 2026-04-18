namespace CanisUIForge.Avalonia.ViewModels;

public class ProjectSetupViewModel : ViewModelBase
{
    public string SolutionName { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string NamespaceRoot { get; set; } = string.Empty;

    public bool TargetBlazor { get; set; }

    public bool TargetMaui { get; set; }

    public bool EnableUnitTests { get; set; }

    public bool EnablePlaywrightTests { get; set; }

    public bool EnableAppiumTests { get; set; }

    public bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(SolutionName))
        {
            AddError("Solution name is required.");
        }

        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            AddError("Output path is required.");
        }

        if (!TargetBlazor && !TargetMaui)
        {
            AddError("At least one target platform must be selected.");
        }

        return !HasErrors;
    }

    public void SyncFromState(WizardState state)
    {
        SolutionName = state.SolutionName;
        OutputPath = state.OutputPath;
        NamespaceRoot = state.NamespaceRoot;
        TargetBlazor = state.Targets.Contains(TargetPlatform.Blazor) || state.Targets.Contains(TargetPlatform.Both);
        TargetMaui = state.Targets.Contains(TargetPlatform.Maui) || state.Targets.Contains(TargetPlatform.Both);
        EnableUnitTests = state.EnableUnitTests;
        EnablePlaywrightTests = state.EnablePlaywrightTests;
        EnableAppiumTests = state.EnableAppiumTests;
    }

    public void SyncToState(WizardState state)
    {
        state.SolutionName = SolutionName;
        state.OutputPath = OutputPath;
        state.NamespaceRoot = NamespaceRoot;
        state.EnableUnitTests = EnableUnitTests;
        state.EnablePlaywrightTests = EnablePlaywrightTests;
        state.EnableAppiumTests = EnableAppiumTests;

        state.Targets.Clear();

        if (TargetBlazor && TargetMaui)
        {
            state.Targets.Add(TargetPlatform.Both);
        }
        else if (TargetBlazor)
        {
            state.Targets.Add(TargetPlatform.Blazor);
        }
        else if (TargetMaui)
        {
            state.Targets.Add(TargetPlatform.Maui);
        }
    }
}
