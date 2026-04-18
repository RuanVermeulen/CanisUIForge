namespace CanisUIForge.Generation.Output;

public interface IRegenerationTracker
{
    void Record(string filePath, RegenerationAction action);

    RegenerationResult GetResult();

    void Reset();
}
