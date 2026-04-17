namespace CanisUIForge.Core.Configuration;

public interface IConfigLoader
{
    Task<ForgeConfig> LoadAsync(string filePath);
}
