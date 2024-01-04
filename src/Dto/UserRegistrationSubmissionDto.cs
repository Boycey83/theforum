namespace theforum.Dto;

public record UserRegistrationSubmissionDto(
    string EmailAddress,
    string Username,
    string Password,
    string PasswordConfirm);
    