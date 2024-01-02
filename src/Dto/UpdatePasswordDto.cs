namespace theforum.Dto;

public class UpdatePasswordDto
{
    public string EmailAddress { get; set; } = string.Empty;
    public string AuthenticationCode { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;
}