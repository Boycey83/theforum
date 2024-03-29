using Microsoft.AspNetCore.Html;
using System.Text;

namespace theforum.Settings;

public record StyleSettings
{
    public string PrimaryBgColor { get; init; } = string.Empty;
    public string PrimaryAccentColor { get; init; } = string.Empty;
    public string SecondaryAccentColor { get; init; } = string.Empty;
    public string BodyTextColor { get; init; } = string.Empty;
    public string DarkTextColor { get; init; } = string.Empty;
    public string SectionBgColor { get; init; } = string.Empty;
    public string DividerColor { get; init; } = string.Empty;
    public string NeutralAccentColor { get; init; } = string.Empty;
    public string SoftDividerColor { get; init; } = string.Empty;
    public string LightAccentColor { get; init; } = string.Empty;
    public string MutedAccentColor { get; init; } = string.Empty;
    public string DarkSectionBgColor { get; init; } = string.Empty;

    public IHtmlContent AsCss => RenderStylesAsCssVariables();

    private IHtmlContent RenderStylesAsCssVariables()
    {
        var sb = new StringBuilder(":root {");
        AddCssVariable(sb, "primary-bg-color", PrimaryBgColor);
        AddCssVariable(sb, "primary-accent-color", PrimaryAccentColor);
        AddCssVariable(sb, "secondary-accent-color", SecondaryAccentColor);
        AddCssVariable(sb, "body-text-color", BodyTextColor);
        AddCssVariable(sb, "dark-text-color", DarkTextColor);
        AddCssVariable(sb, "section-bg-color", SectionBgColor);
        AddCssVariable(sb, "divider-color", DividerColor);
        AddCssVariable(sb, "neutral-accent-color", NeutralAccentColor);
        AddCssVariable(sb, "soft-divider-color", SoftDividerColor);
        AddCssVariable(sb, "light-accent-color", LightAccentColor);
        AddCssVariable(sb, "muted-accent-color", MutedAccentColor);
        AddCssVariable(sb, "dark-section-bg-color", DarkSectionBgColor);
        sb.Append('}');
        return new HtmlString(sb.ToString());
    }

    private static void AddCssVariable(StringBuilder sb, string name, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            sb.AppendLine($" --{name}: {value};");
        }
    }
}
