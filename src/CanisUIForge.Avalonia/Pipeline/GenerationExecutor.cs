namespace CanisUIForge.Avalonia.Pipeline;

public class GenerationExecutor
{
    private readonly IBlazorFoundationGenerator _blazorFoundation;
    private readonly IBlazorComponentsGenerator _blazorComponents;
    private readonly IBlazorPageGenerator _blazorPages;
    private readonly IBlazorApiServiceGenerator _blazorApiServices;
    private readonly IMauiFoundationGenerator _mauiFoundation;
    private readonly IMauiComponentsGenerator _mauiComponents;
    private readonly IMauiPageGenerator _mauiPages;
    private readonly IUnitTestGenerator _unitTests;
    private readonly IPlaywrightTestGenerator _playwrightTests;
    private readonly IAppiumTestGenerator _appiumTests;

    public GenerationExecutor(
        IBlazorFoundationGenerator blazorFoundation,
        IBlazorComponentsGenerator blazorComponents,
        IBlazorPageGenerator blazorPages,
        IBlazorApiServiceGenerator blazorApiServices,
        IMauiFoundationGenerator mauiFoundation,
        IMauiComponentsGenerator mauiComponents,
        IMauiPageGenerator mauiPages,
        IUnitTestGenerator unitTests,
        IPlaywrightTestGenerator playwrightTests,
        IAppiumTestGenerator appiumTests)
    {
        _blazorFoundation = blazorFoundation ?? throw new ArgumentNullException(nameof(blazorFoundation));
        _blazorComponents = blazorComponents ?? throw new ArgumentNullException(nameof(blazorComponents));
        _blazorPages = blazorPages ?? throw new ArgumentNullException(nameof(blazorPages));
        _blazorApiServices = blazorApiServices ?? throw new ArgumentNullException(nameof(blazorApiServices));
        _mauiFoundation = mauiFoundation ?? throw new ArgumentNullException(nameof(mauiFoundation));
        _mauiComponents = mauiComponents ?? throw new ArgumentNullException(nameof(mauiComponents));
        _mauiPages = mauiPages ?? throw new ArgumentNullException(nameof(mauiPages));
        _unitTests = unitTests ?? throw new ArgumentNullException(nameof(unitTests));
        _playwrightTests = playwrightTests ?? throw new ArgumentNullException(nameof(playwrightTests));
        _appiumTests = appiumTests ?? throw new ArgumentNullException(nameof(appiumTests));
    }

    public event Action<string>? StepStarted;

    public async Task ExecuteAsync(GenerationPlan plan)
    {
        bool generateBlazor = plan.Targets.Contains(TargetPlatform.Blazor)
            || plan.Targets.Contains(TargetPlatform.Both);

        bool generateMaui = plan.Targets.Contains(TargetPlatform.Maui)
            || plan.Targets.Contains(TargetPlatform.Both);

        if (generateBlazor)
        {
            StepStarted?.Invoke("Generating Blazor foundation...");
            await _blazorFoundation.GenerateAsync(plan);

            StepStarted?.Invoke("Generating Blazor components...");
            await _blazorComponents.GenerateAsync(plan);

            StepStarted?.Invoke("Generating Blazor API services...");
            await _blazorApiServices.GenerateAsync(plan);

            StepStarted?.Invoke("Generating Blazor pages...");
            await _blazorPages.GenerateAsync(plan);
        }

        if (generateMaui)
        {
            StepStarted?.Invoke("Generating MAUI foundation...");
            await _mauiFoundation.GenerateAsync(plan);

            StepStarted?.Invoke("Generating MAUI components...");
            await _mauiComponents.GenerateAsync(plan);

            StepStarted?.Invoke("Generating MAUI pages...");
            await _mauiPages.GenerateAsync(plan);
        }

        if (plan.Tests.Unit)
        {
            StepStarted?.Invoke("Generating unit tests...");
            await _unitTests.GenerateAsync(plan);
        }

        if (plan.Tests.Playwright && generateBlazor)
        {
            StepStarted?.Invoke("Generating Playwright tests...");
            await _playwrightTests.GenerateAsync(plan);
        }

        if (plan.Tests.Appium && generateMaui)
        {
            StepStarted?.Invoke("Generating Appium tests...");
            await _appiumTests.GenerateAsync(plan);
        }
    }
}
