namespace theforum.Model;

public class UserAccount
{
    public UserAccount()
    {
    }

    public UserAccount(string emailAddress, string username, string passwordSalt, string passwordHash, Guid token)
    {
        EmailAddress = emailAddress;
        Username = username;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Token = token;
    }

    public virtual int Id { get; set; }
    public virtual string Username { get; set; } = string.Empty;
    public virtual string EmailAddress { get; set; } = string.Empty;
    public virtual string PasswordSalt { get; set; } = string.Empty;
    public virtual string PasswordHash { get; set; } = string.Empty;
    public virtual Guid Token { get; set; }
    public virtual bool IsActivated { get; set; }
    public virtual Guid? ResetToken { get; set; }
    public virtual DateTime? ResetTokenExpiry { get; set; }
}