namespace CanisUIForge.Core.Configuration;

public interface IConfigSaver
{
    Task SaveAsync(ForgeConfig config, string filePath);
}
