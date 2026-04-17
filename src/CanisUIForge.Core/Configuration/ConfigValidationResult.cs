namespace CanisUIForge.Core.Configuration;

public class ConfigValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; } = new List<string>();

    public void AddError(string error)
    {
        Errors.Add(error);
    }
}
