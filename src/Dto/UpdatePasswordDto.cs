namespace theforum.Dto;

public record UpdatePasswordDto(
    string EmailAddress,
    string AuthenticationCode,
    string Password,
    string PasswordConfirm);
    