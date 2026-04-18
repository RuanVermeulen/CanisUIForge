namespace CanisUIForge.Avalonia.Services;

public class AvaloniaServiceFactory
{
    private readonly FileWriter _fileWriter = new FileWriter();
    private readonly TemplateEngine _templateEngine = new TemplateEngine();

    public WizardViewModel CreateWizardViewModel()
    {
        IOpenApiScanner scanner = CreateOpenApiScanner();
        IContractsResolver contractsResolver = CreateContractsResolver();
        IConfigValidator configValidator = new ConfigValidator();
        IGenerationPlanBuilder planBuilder = CreatePlanBuilder();

        ProjectSetupViewModel projectSetup = new ProjectSetupViewModel();
        SwaggerInputViewModel swaggerInput = new SwaggerInputViewModel(scanner);
        ControllerSelectionViewModel controllerSelection = new ControllerSelectionViewModel();
        PreviewViewModel preview = new PreviewViewModel(configValidator, scanner, contractsResolver, planBuilder);
        GenerationViewModel generation = new GenerationViewModel(CreateGenerationExecutor());

        return new WizardViewModel(projectSetup, swaggerInput, controllerSelection, preview, generation);
    }

    private IOpenApiScanner CreateOpenApiScanner()
    {
        SwaggerLoader loader = new SwaggerLoader();
        EndpointClassifier classifier = new EndpointClassifier();
        return new OpenApiScanner(loader, classifier);
    }

    private IContractsResolver CreateContractsResolver()
    {
        SchemaTypeMapper mapper = new SchemaTypeMapper();
        return new ContractsResolver(mapper);
    }

    private IGenerationPlanBuilder CreatePlanBuilder()
    {
        MetadataUnifier unifier = new MetadataUnifier();
        return new GenerationPlanBuilder(unifier);
    }

    private GenerationExecutor CreateGenerationExecutor()
    {
        return new GenerationExecutor(
            CreateBlazorFoundationGenerator(),
            CreateBlazorComponentsGenerator(),
            CreateBlazorPageGenerator(),
            CreateBlazorApiServiceGenerator(),
            CreateMauiFoundationGenerator(),
            CreateMauiComponentsGenerator(),
            CreateMauiPageGenerator(),
            CreateUnitTestGenerator(),
            CreatePlaywrightTestGenerator(),
            CreateAppiumTestGenerator());
    }

    private ITemplateLoader CreateBlazorTemplateLoader()
    {
        Assembly assembly = typeof(BlazorFoundationGenerator).Assembly;
        return new EmbeddedResourceTemplateLoader(assembly, "CanisUIForge.Blazor.Templates");
    }

    private ITemplateLoader CreateMauiTemplateLoader()
    {
        Assembly assembly = typeof(MauiFoundationGenerator).Assembly;
        return new EmbeddedResourceTemplateLoader(assembly, "CanisUIForge.Maui.Templates");
    }

    private ITemplateLoader CreateTestingTemplateLoader()
    {
        Assembly assembly = typeof(UnitTestGenerator).Assembly;
        return new EmbeddedResourceTemplateLoader(assembly, "CanisUIForge.Testing.Templates");
    }

    private IBlazorFoundationGenerator CreateBlazorFoundationGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorFoundationGenerator(
            new BlazorProjectGenerator(_fileWriter, _templateEngine, loader),
            new BlazorBootstrapGenerator(_fileWriter, _templateEngine, loader),
            new LayoutGenerator(_fileWriter, _templateEngine, loader),
            new NavigationGenerator(_fileWriter, _templateEngine, loader),
            new HomePageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IBlazorComponentsGenerator CreateBlazorComponentsGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorComponentsGenerator(
            new DataGridGenerator(_fileWriter, _templateEngine, loader),
            new DataGridColumnGenerator(_fileWriter, _templateEngine, loader),
            new FormGenerator(_fileWriter, _templateEngine, loader),
            new FieldRendererGenerator(_fileWriter, _templateEngine, loader),
            new SearchPanelGenerator(_fileWriter, _templateEngine, loader),
            new LoadingPanelGenerator(_fileWriter, _templateEngine, loader),
            new ErrorPanelGenerator(_fileWriter, _templateEngine, loader),
            new EmptyStateGenerator(_fileWriter, _templateEngine, loader),
            new DialogSystemGenerator(_fileWriter, _templateEngine, loader),
            new StylingGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IBlazorPageGenerator CreateBlazorPageGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorPageGenerator(
            new ListPageGenerator(_fileWriter, _templateEngine, loader),
            new CreatePageGenerator(_fileWriter, _templateEngine, loader),
            new EditPageGenerator(_fileWriter, _templateEngine, loader),
            new SearchPageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IBlazorApiServiceGenerator CreateBlazorApiServiceGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorApiServiceGenerator(
            new ApiServiceInterfaceGenerator(_fileWriter, _templateEngine, loader),
            new ApiServiceImplementationGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IMauiFoundationGenerator CreateMauiFoundationGenerator()
    {
        ITemplateLoader loader = CreateMauiTemplateLoader();

        return new MauiFoundationGenerator(
            new MauiProjectGenerator(_fileWriter, _templateEngine, loader),
            new MauiBootstrapGenerator(_fileWriter, _templateEngine, loader),
            new MauiShellGenerator(_fileWriter, _templateEngine, loader),
            new MauiHomePageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IMauiComponentsGenerator CreateMauiComponentsGenerator()
    {
        ITemplateLoader loader = CreateMauiTemplateLoader();

        return new MauiComponentsGenerator(
            new MauiFormLayoutGenerator(_fileWriter, _templateEngine, loader),
            new MauiCollectionListGenerator(_fileWriter, _templateEngine, loader),
            new MauiSearchBarViewGenerator(_fileWriter, _templateEngine, loader),
            new MauiLoadingIndicatorGenerator(_fileWriter, _templateEngine, loader),
            new MauiErrorViewGenerator(_fileWriter, _templateEngine, loader),
            new MauiEmptyStateViewGenerator(_fileWriter, _templateEngine, loader),
            new MauiDialogServiceGenerator(_fileWriter, _templateEngine, loader),
            new MauiStylingGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IMauiPageGenerator CreateMauiPageGenerator()
    {
        ITemplateLoader loader = CreateMauiTemplateLoader();

        return new MauiPageGenerator(
            new MauiListPageGenerator(_fileWriter, _templateEngine, loader),
            new MauiCreatePageGenerator(_fileWriter, _templateEngine, loader),
            new MauiEditPageGenerator(_fileWriter, _templateEngine, loader),
            new MauiSearchPageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IUnitTestGenerator CreateUnitTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new UnitTestGenerator(
            new TestProjectGenerator(_fileWriter, _templateEngine, loader),
            new ApiServiceTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IPlaywrightTestGenerator CreatePlaywrightTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new BlazorPlaywrightTestGenerator(
            new PlaywrightProjectGenerator(_fileWriter, _templateEngine, loader),
            new PlaywrightTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    private IAppiumTestGenerator CreateAppiumTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new MauiAppiumTestGenerator(
            new AppiumProjectGenerator(_fileWriter, _templateEngine, loader),
            new AppiumTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }
}
