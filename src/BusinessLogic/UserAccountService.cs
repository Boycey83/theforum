using System.ComponentModel.DataAnnotations;
using theforum.DataAccess;
using theforum.Model;
using theforum.Resources;

namespace theforum.BusinessLogic;

public class UserAccountService
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly EmailHelper _emailHelper;

    public UserAccountService(IUserAccountRepository userAccountRepository, EmailHelper emailHelper)
    {
        _userAccountRepository = userAccountRepository;
        _emailHelper = emailHelper;
    }

    public int CreateUser(string userEmail, string username, string password, string passwordConfirm)
    {
        if (!string.IsNullOrEmpty(userEmail))
        {
            userEmail = userEmail.Trim();
        }

        if (!string.IsNullOrEmpty(username))
        {
            username = username.Trim();
        }

        ValidateCreate(userEmail, username, password, passwordConfirm);
        var passwordSalt = AuthenticationHelper.GetPasswordSalt();
        var passwordHash = AuthenticationHelper.GetPasswordHash(password, passwordSalt);
        var token = Guid.NewGuid();
        var userAccount = new UserAccount(userEmail, username, passwordSalt, passwordHash, token);
        var userAccountId = _userAccountRepository.CreateUser(userAccount);
        _emailHelper.SendUserRegistrationEmail(userEmail, username, userAccountId, token);
        return userAccountId;
    }

    public bool VerifyPasswordResetEmail(string emailAddress)
    {
        emailAddress = emailAddress.Trim();
        var userAccount = _userAccountRepository.GetByEmail(emailAddress);
        return userAccount != null;
    }

    public bool RequestPasswordReset(string emailAddress)
    {
        emailAddress = emailAddress.Trim();
        var userAccount = _userAccountRepository.GetByEmail(emailAddress);
        if (userAccount == null)
        {
            return false;
        }
        userAccount.ResetToken = Guid.NewGuid();
        userAccount.ResetTokenExpiry = DateTime.UtcNow.AddDays(1);
        _userAccountRepository.UpdateUser(userAccount);
        _emailHelper.SendUserPasswordResetEmail(userAccount);
        return true;
    }

    public void UpdatePassword(string emailAddress, string authenticationCode, string password, string passwordConfirm)
    {
        emailAddress = emailAddress.Trim();
        var userAccount = _userAccountRepository.GetByEmail(emailAddress);
        if (userAccount == null)
        {
            throw new ArgumentException($"No matching user account found with email: {emailAddress}");
        }
        ValidateUpdatePassword(authenticationCode, password, passwordConfirm, userAccount);
        userAccount.PasswordSalt = AuthenticationHelper.GetPasswordSalt();
        userAccount.PasswordHash = AuthenticationHelper.GetPasswordHash(password, userAccount.PasswordSalt);
        userAccount.ResetTokenExpiry = null;
        userAccount.ResetToken = null;
        _userAccountRepository.UpdateUser(userAccount);
    }

    public int ValidateUser(string username, string password)
    {
        var userAccount =
            GetByUserAccountByUsernameAndValidateUsername(username, ExceptionMessages.ValidateUsernameNotFound);
        ValidateAccountIsActivated(userAccount);
        if (AuthenticationHelper.ValidatePassword(userAccount.PasswordSalt, userAccount.PasswordHash, password))
        {
            return userAccount.Id;
        }

        return 0;
    }

    public bool ActivateUser(int id, Guid token)
    {
        var userAccount = GetByUserAccountByIdAndValidateId(id);
        if (userAccount.Token == token)
        {
            _userAccountRepository.ActivateAccount(id);
            return true;
        }

        return false;
    }

    #region Validation Methods

    private UserAccount GetByUserAccountByUsernameAndValidateUsername(string username, string exceptionMessage)
    {
        var userAccount = _userAccountRepository.GetByUsername(username);
        if (userAccount == null)
        {
            throw new ValidationException(string.Format(exceptionMessage, username));
        }
        return userAccount;
    }

    private UserAccount GetByUserAccountByIdAndValidateId(int userId)
    {
        var userAccount = _userAccountRepository.GetById(userId);
        if (userAccount == null)
        {
            throw new ValidationException(string.Format(ExceptionMessages.UserAccountUserIdNotFound));
        }
        return userAccount;
    }

    private void ValidateAccountIsActivated(UserAccount userAccount)
    {
        if (!userAccount.IsActivated)
        {
            throw new ValidationException(string.Format(ExceptionMessages.UserAccountNotActivated,
                userAccount.EmailAddress));
        }
    }

    private void ValidateCreate(string userEmail, string username, string password, string passwordConfirm)
    {
        ValidateUserAccountEmail(userEmail);
        ValidateUserAccountUsername(username);
        ValidateUserAccountPassword(password, passwordConfirm);
    }

    private void ValidateUserAccountPassword(string password, string passwordConfirm)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountPasswordMustBeSupplied);
        }

        if (password != passwordConfirm)
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountPasswordsMustMatch);
        }

        if (password.Length < 8)
        {
            throw new ValidationException(ExceptionMessages.UserAccountPasswordTooShort);
        }
    }

    private void ValidateUserAccountEmail(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmptyEmailAddress);
        }

        if (userEmail.Length > 256)
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmailAddressTooLong);
        }

        if (!userEmail.Contains("@"))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressNoAtSymbol,
                userEmail));
        }

        if (!userEmail.Contains("."))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressNoDotSymbol,
                userEmail));
        }

        ValidateDuplicateEmail(userEmail);
    }

    private void ValidateUserAccountUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmptyUsername);
        }

        if (username.Length > 30)
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountUsernameTooLong);
        }

        ValidateDuplicateUsername(username);
    }

    private void ValidateDuplicateEmail(string userEmail)
    {
        if (_userAccountRepository.ExistsWithEmail(userEmail))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressDuplicate,
                userEmail));
        }
    }

    private void ValidateDuplicateUsername(string username)
    {
        if (_userAccountRepository.ExistsWithUsername(username))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountUsernameDuplicate,
                username));
        }
    }

    private static void ValidateUpdatePassword(string authenticationCode, string password, string passwordConfirm,
        UserAccount userAccount)
    {
        if (userAccount == null)
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordUserAccountNotFound);
        }

        if (userAccount.ResetTokenExpiry < DateTime.UtcNow)
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordTokenExpired);
        }

        if (authenticationCode == null || userAccount.ResetToken.ToString() != authenticationCode.ToLower())
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordTokenIncorrect);
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordPasswordMustBeSupplied);
        }

        if (password != passwordConfirm)
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordPasswordsMustMatch);
        }

        if (password.Length < 8)
        {
            throw new ValidationException(ExceptionMessages.UpdatePasswordPasswordTooShort);
        }
    }

    #endregion
}