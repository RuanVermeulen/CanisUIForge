namespace CanisUIForge.Avalonia.ViewModels;

public class GenerationViewModel : ViewModelBase
{
    private readonly GenerationExecutor _executor;
    private readonly IRegenerationTracker _tracker;
    private readonly IForgeLogger _logger;

    public GenerationViewModel(GenerationExecutor executor, IRegenerationTracker tracker, IForgeLogger logger)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IReadOnlyList<ForgeLogEntry> LogEntries => _logger.Entries;

    public bool IsGenerating { get; private set; }

    public bool IsComplete { get; private set; }

    public bool HasFailed { get; private set; }

    public event Action? LogUpdated;

    public event Action? GenerationCompleted;

    public async Task ExecuteAsync(GenerationPlan plan)
    {
        ClearErrors();
        _logger.Clear();
        IsGenerating = true;
        IsComplete = false;
        HasFailed = false;

        void OnStepStarted(string message)
        {
            _logger.Log(ForgeLogLevel.Information, message, "Generation");
        }

        void OnEntryLogged(ForgeLogEntry entry)
        {
            LogUpdated?.Invoke();
        }

        try
        {
            _logger.EntryLogged += OnEntryLogged;
            _logger.Log(ForgeLogLevel.Information, "Starting generation...", "Generation");

            _tracker.Reset();
            _executor.StepStarted += OnStepStarted;
            await _executor.ExecuteAsync(plan);

            RegenerationResult result = _tracker.GetResult();
            _logger.Log(ForgeLogLevel.Information, $"Files created: {result.CreatedCount}, overwritten: {result.OverwrittenCount}, skipped: {result.SkippedCount}", "Summary");

            if (result.SkippedCount > 0)
            {
                _logger.Log(ForgeLogLevel.Warning, $"{result.SkippedCount} file(s) skipped (manual modifications preserved)", "Summary");
            }

            _logger.Log(ForgeLogLevel.Success, "Generation completed successfully!");
            IsComplete = true;
        }
        catch (Exception exception)
        {
            _logger.Log(ForgeLogLevel.Error, $"Generation failed: {exception.Message}", "Generation");
            HasFailed = true;
            AddError(exception.Message);
        }
        finally
        {
            _executor.StepStarted -= OnStepStarted;
            _logger.EntryLogged -= OnEntryLogged;
            IsGenerating = false;
            GenerationCompleted?.Invoke();
        }
    }
}
