namespace theforum.Settings;

public record EmailSettings
{
    public string SenderName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ForumHostName { get; init; } = string.Empty;
}