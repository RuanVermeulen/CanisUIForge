namespace CanisUIForge.Cli.Pipeline;

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

    public async Task ExecuteAsync(GenerationPlan plan)
    {
        bool generateBlazor = plan.Targets.Contains(TargetPlatform.Blazor)
            || plan.Targets.Contains(TargetPlatform.Both);

        bool generateMaui = plan.Targets.Contains(TargetPlatform.Maui)
            || plan.Targets.Contains(TargetPlatform.Both);

        if (generateBlazor)
        {
            Console.WriteLine("  Generating Blazor application...");
            await _blazorFoundation.GenerateAsync(plan);
            await _blazorComponents.GenerateAsync(plan);
            await _blazorApiServices.GenerateAsync(plan);
            await _blazorPages.GenerateAsync(plan);
        }

        if (generateMaui)
        {
            Console.WriteLine("  Generating MAUI application...");
            await _mauiFoundation.GenerateAsync(plan);
            await _mauiComponents.GenerateAsync(plan);
            await _mauiPages.GenerateAsync(plan);
        }

        if (plan.Tests.Unit)
        {
            Console.WriteLine("  Generating unit tests...");
            await _unitTests.GenerateAsync(plan);
        }

        if (plan.Tests.Playwright && generateBlazor)
        {
            Console.WriteLine("  Generating Playwright tests...");
            await _playwrightTests.GenerateAsync(plan);
        }

        if (plan.Tests.Appium && generateMaui)
        {
            Console.WriteLine("  Generating Appium tests...");
            await _appiumTests.GenerateAsync(plan);
        }
    }
}
