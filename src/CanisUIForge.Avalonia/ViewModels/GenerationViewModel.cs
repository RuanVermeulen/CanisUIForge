namespace CanisUIForge.Avalonia.ViewModels;

public class GenerationViewModel : ViewModelBase
{
    private readonly GenerationExecutor _executor;
    private readonly IRegenerationTracker _tracker;

    public GenerationViewModel(GenerationExecutor executor, IRegenerationTracker tracker)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    }

    public List<GenerationLogEntry> LogEntries { get; } = new List<GenerationLogEntry>();

    public bool IsGenerating { get; private set; }

    public bool IsComplete { get; private set; }

    public bool HasFailed { get; private set; }

    public event Action? LogUpdated;

    public event Action? GenerationCompleted;

    public async Task ExecuteAsync(GenerationPlan plan)
    {
        ClearErrors();
        LogEntries.Clear();
        IsGenerating = true;
        IsComplete = false;
        HasFailed = false;

        void OnStepStarted(string message)
        {
            AppendLog(message, GenerationLogLevel.Info);
        }

        try
        {
            AppendLog("Starting generation...", GenerationLogLevel.Info);

            _tracker.Reset();
            _executor.StepStarted += OnStepStarted;
            await _executor.ExecuteAsync(plan);

            RegenerationResult result = _tracker.GetResult();
            AppendLog($"Files created: {result.CreatedCount}, overwritten: {result.OverwrittenCount}, skipped: {result.SkippedCount}", GenerationLogLevel.Info);

            if (result.SkippedCount > 0)
            {
                AppendLog($"{result.SkippedCount} file(s) skipped (manual modifications preserved)", GenerationLogLevel.Warning);
            }

            AppendLog("Generation completed successfully!", GenerationLogLevel.Success);
            IsComplete = true;
        }
        catch (Exception exception)
        {
            AppendLog($"Generation failed: {exception.Message}", GenerationLogLevel.Error);
            HasFailed = true;
            AddError(exception.Message);
        }
        finally
        {
            _executor.StepStarted -= OnStepStarted;
            IsGenerating = false;
            GenerationCompleted?.Invoke();
        }
    }

    private void AppendLog(string message, GenerationLogLevel level)
    {
        GenerationLogEntry entry = new GenerationLogEntry
        {
            Timestamp = DateTime.Now,
            Message = message,
            Level = level
        };

        LogEntries.Add(entry);
        LogUpdated?.Invoke();
    }
}
