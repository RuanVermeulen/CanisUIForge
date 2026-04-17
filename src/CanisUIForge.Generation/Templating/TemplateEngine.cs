using System.Text;

namespace CanisUIForge.Generation.Templating;

public class TemplateEngine : ITemplateEngine
{
    private const string PlaceholderPrefix = "{{";
    private const string PlaceholderSuffix = "}}";

    public string Render(string template, Dictionary<string, string> replacements)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        if (replacements is null)
        {
            throw new ArgumentNullException(nameof(replacements));
        }

        string result = template;

        foreach (KeyValuePair<string, string> replacement in replacements)
        {
            string placeholder = $"{PlaceholderPrefix}{replacement.Key}{PlaceholderSuffix}";
            result = result.Replace(placeholder, replacement.Value);
        }

        return result;
    }
}
