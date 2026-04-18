namespace CanisUIForge.Generation.Validation;

public class PipelineValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; } = new List<string>();

    public List<string> Warnings { get; } = new List<string>();

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}
