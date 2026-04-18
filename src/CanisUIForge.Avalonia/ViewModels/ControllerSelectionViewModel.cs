namespace CanisUIForge.Avalonia.ViewModels;

public class ControllerSelectionViewModel : ViewModelBase
{
    public List<ControllerSelection> Controllers { get; set; } = new List<ControllerSelection>();

    public void LoadFromApiDefinition(ApiDefinition apiDefinition)
    {
        Controllers.Clear();

        foreach (ResourceDefinition resource in apiDefinition.Resources)
        {
            ControllerSelection selection = new ControllerSelection
            {
                Name = resource.Name,
                IsSelected = true,
                Style = GenerationStyle.FormAndGrid,
                EndpointCount = resource.Endpoints.Count
            };

            Controllers.Add(selection);
        }
    }

    public void SelectAll()
    {
        foreach (ControllerSelection controller in Controllers)
        {
            controller.IsSelected = true;
        }
    }

    public void DeselectAll()
    {
        foreach (ControllerSelection controller in Controllers)
        {
            controller.IsSelected = false;
        }
    }

    public bool Validate()
    {
        ClearErrors();

        bool anySelected = Controllers.Any(controller => controller.IsSelected);

        if (!anySelected)
        {
            AddError("At least one controller must be selected.");
        }

        return !HasErrors;
    }

    public void SyncFromState(WizardState state)
    {
        if (state.ControllerSelections.Count > 0)
        {
            Controllers = new List<ControllerSelection>(state.ControllerSelections);
        }
    }

    public void SyncToState(WizardState state)
    {
        state.ControllerSelections = new List<ControllerSelection>(Controllers);
    }
}
