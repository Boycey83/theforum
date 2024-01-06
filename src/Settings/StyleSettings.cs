using Microsoft.AspNetCore.Html;

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

    public IHtmlContent AsCss =>
        new HtmlString(
            $$"""
              :root {
                  --primary-accent-color: {{PrimaryAccentColor}};
                  --secondary-accent-color: {{SecondaryAccentColor}};
                  --body-text-color: {{BodyTextColor}};
                  --dark-text-color: {{DarkTextColor}};
                  --section-bg-color: {{SectionBgColor}};
                  --primary-bg-color: {{PrimaryBgColor}};
                  --divider-color: {{DividerColor}};
                  --neutral-accent-color: {{NeutralAccentColor}};
                  --soft-divider-color: {{SoftDividerColor}};
                  --light-accent-color: {{LightAccentColor}};
                  --muted-accent-color: {{MutedAccentColor}};
                  --dark-section-bg-color: {{DarkSectionBgColor}};
              }
              """);
}
