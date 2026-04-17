namespace CanisUIForge.Generation.Templating;

public interface ITemplateEngine
{
    string Render(string template, Dictionary<string, string> replacements);
}
