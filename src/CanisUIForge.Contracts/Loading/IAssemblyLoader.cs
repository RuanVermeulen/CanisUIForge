namespace CanisUIForge.Contracts.Loading;

public interface IAssemblyLoader
{
    Task<Assembly> LoadAsync();
}
