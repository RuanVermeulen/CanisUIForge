namespace CanisUIForge.Avalonia.Models;

public class ControllerSelection
{
    public string Name { get; set; } = string.Empty;

    public bool IsSelected { get; set; } = true;

    public GenerationStyle Style { get; set; } = GenerationStyle.FormAndGrid;

    public int EndpointCount { get; set; }
}
