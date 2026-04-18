namespace CanisUIForge.Avalonia.Navigation;

public class WizardNavigator
{
    private static readonly WizardStep[] Steps = new WizardStep[]
    {
        WizardStep.ProjectSetup,
        WizardStep.SwaggerInput,
        WizardStep.ControllerSelection,
        WizardStep.Preview,
        WizardStep.Generation
    };

    private int _currentIndex;

    public WizardStep CurrentStep => Steps[_currentIndex];

    public bool CanGoNext => _currentIndex < Steps.Length - 1;

    public bool CanGoBack => _currentIndex > 0;

    public WizardStep GoNext()
    {
        if (!CanGoNext)
        {
            throw new InvalidOperationException("Already at the last step.");
        }

        _currentIndex++;
        return CurrentStep;
    }

    public WizardStep GoBack()
    {
        if (!CanGoBack)
        {
            throw new InvalidOperationException("Already at the first step.");
        }

        _currentIndex--;
        return CurrentStep;
    }

    public void GoTo(WizardStep step)
    {
        int index = Array.IndexOf(Steps, step);

        if (index < 0)
        {
            throw new ArgumentException($"Unknown wizard step: {step}", nameof(step));
        }

        _currentIndex = index;
    }
}
