namespace CanisUIForge.Avalonia.ViewModels;

public class ViewModelBase
{
    private readonly List<string> _errors = new List<string>();

    public IReadOnlyList<string> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;

    public event Action? ErrorsChanged;

    protected void ClearErrors()
    {
        _errors.Clear();
        ErrorsChanged?.Invoke();
    }

    protected void AddError(string error)
    {
        _errors.Add(error);
        ErrorsChanged?.Invoke();
    }
}
