namespace CanisUIForge.Avalonia.ViewModels;

public class GenerationViewModel : ViewModelBase
{
    private readonly GenerationExecutor _executor;

    public GenerationViewModel(GenerationExecutor executor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
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

            _executor.StepStarted += OnStepStarted;
            await _executor.ExecuteAsync(plan);

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
