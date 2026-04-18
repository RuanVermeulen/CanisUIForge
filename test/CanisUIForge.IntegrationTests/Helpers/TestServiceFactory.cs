namespace CanisUIForge.IntegrationTests.Helpers;

public class TestServiceFactory
{
    private readonly RegenerationTracker _tracker;
    private readonly ForgeLogger _logger;
    private readonly FileWriter _fileWriter;
    private readonly TemplateEngine _templateEngine;

    public TestServiceFactory()
    {
        _tracker = new RegenerationTracker();
        _logger = new ForgeLogger();
        _fileWriter = new FileWriter(_tracker);
        _templateEngine = new TemplateEngine();
    }

    public IRegenerationTracker Tracker => _tracker;

    public IForgeLogger Logger => _logger;

    public IFileWriter FileWriter => _fileWriter;

    public IOpenApiScanner CreateOpenApiScanner()
    {
        SwaggerLoader loader = new SwaggerLoader();
        EndpointClassifier classifier = new EndpointClassifier();
        return new OpenApiScanner(loader, classifier);
    }

    public IContractsResolver CreateContractsResolver()
    {
        SchemaTypeMapper mapper = new SchemaTypeMapper();
        return new ContractsResolver(mapper);
    }

    public IGenerationPlanBuilder CreatePlanBuilder()
    {
        MetadataUnifier unifier = new MetadataUnifier();
        return new GenerationPlanBuilder(unifier);
    }

    public IPipelineValidator CreatePipelineValidator()
    {
        return new PipelineValidator();
    }

    public IConfigValidator CreateConfigValidator()
    {
        return new ConfigValidator();
    }

    public IBlazorFoundationGenerator CreateBlazorFoundationGenerator()
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

    public IBlazorComponentsGenerator CreateBlazorComponentsGenerator()
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

    public IBlazorPageGenerator CreateBlazorPageGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorPageGenerator(
            new ListPageGenerator(_fileWriter, _templateEngine, loader),
            new CreatePageGenerator(_fileWriter, _templateEngine, loader),
            new EditPageGenerator(_fileWriter, _templateEngine, loader),
            new SearchPageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IBlazorApiServiceGenerator CreateBlazorApiServiceGenerator()
    {
        ITemplateLoader loader = CreateBlazorTemplateLoader();

        return new BlazorApiServiceGenerator(
            new ApiServiceInterfaceGenerator(_fileWriter, _templateEngine, loader),
            new ApiServiceImplementationGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IMauiFoundationGenerator CreateMauiFoundationGenerator()
    {
        ITemplateLoader loader = CreateMauiTemplateLoader();

        return new MauiFoundationGenerator(
            new MauiProjectGenerator(_fileWriter, _templateEngine, loader),
            new MauiBootstrapGenerator(_fileWriter, _templateEngine, loader),
            new MauiShellGenerator(_fileWriter, _templateEngine, loader),
            new MauiHomePageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IMauiComponentsGenerator CreateMauiComponentsGenerator()
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

    public IMauiPageGenerator CreateMauiPageGenerator()
    {
        ITemplateLoader loader = CreateMauiTemplateLoader();

        return new MauiPageGenerator(
            new MauiListPageGenerator(_fileWriter, _templateEngine, loader),
            new MauiCreatePageGenerator(_fileWriter, _templateEngine, loader),
            new MauiEditPageGenerator(_fileWriter, _templateEngine, loader),
            new MauiSearchPageGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IUnitTestGenerator CreateUnitTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new UnitTestGenerator(
            new TestProjectGenerator(_fileWriter, _templateEngine, loader),
            new ApiServiceTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IPlaywrightTestGenerator CreatePlaywrightTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new BlazorPlaywrightTestGenerator(
            new PlaywrightProjectGenerator(_fileWriter, _templateEngine, loader),
            new PlaywrightTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
    }

    public IAppiumTestGenerator CreateAppiumTestGenerator()
    {
        ITemplateLoader loader = CreateTestingTemplateLoader();

        return new MauiAppiumTestGenerator(
            new AppiumProjectGenerator(_fileWriter, _templateEngine, loader),
            new AppiumTestGenerator(_fileWriter, _templateEngine, loader),
            _fileWriter);
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
}
