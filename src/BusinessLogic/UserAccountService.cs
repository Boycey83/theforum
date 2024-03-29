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

    public async Task<int> CreateUser(string userEmail, string username, string password, string passwordConfirm)
    {
        if (!string.IsNullOrEmpty(userEmail))
        {
            userEmail = userEmail.Trim();
        }

        if (!string.IsNullOrEmpty(username))
        {
            username = username.Trim();
        }

        await ValidateCreate(userEmail, username, password, passwordConfirm);
        var passwordSalt = AuthenticationHelper.GetPasswordSalt();
        var passwordHash = AuthenticationHelper.GetPasswordHash(password, passwordSalt);
        var token = Guid.NewGuid();
        var userAccount = new UserAccount(userEmail, username, passwordSalt, passwordHash, token);
        var userAccountId = await _userAccountRepository.CreateUser(userAccount);
        // TODO: Check if this should be async? feels like it...
        _emailHelper.SendUserRegistrationEmail(userEmail, username, userAccountId, token);
        return userAccountId;
    }

    public async Task<bool> VerifyPasswordResetEmail(string emailAddress)
    {
        emailAddress = emailAddress.Trim();
        var userAccount = await _userAccountRepository.GetByEmail(emailAddress);
        return userAccount != null;
    }

    public async Task<bool> RequestPasswordReset(string emailAddress)
    {
        emailAddress = emailAddress.Trim();
        var userAccount = await _userAccountRepository.GetByEmail(emailAddress);
        if (userAccount == null)
        {
            return false;
        }
        userAccount.ResetToken = Guid.NewGuid();
        userAccount.ResetTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _userAccountRepository.UpdateUser(userAccount);
        // TODO: Check if this should be async? feels like it...
        _emailHelper.SendUserPasswordResetEmail(userAccount);
        return true;
    }

    public async Task UpdatePassword(string emailAddress, string authenticationCode, string password, string passwordConfirm)
    {
        emailAddress = emailAddress.Trim();
        var userAccount =  await _userAccountRepository.GetByEmail(emailAddress);
        if (userAccount == null)
        {
            throw new ArgumentException($"No matching user account found with email: {emailAddress}");
        }
        ValidateUpdatePassword(authenticationCode, password, passwordConfirm, userAccount);
        userAccount.PasswordSalt = AuthenticationHelper.GetPasswordSalt();
        userAccount.PasswordHash = AuthenticationHelper.GetPasswordHash(password, userAccount.PasswordSalt);
        userAccount.ResetTokenExpiry = null;
        userAccount.ResetToken = null;
        await _userAccountRepository.UpdateUser(userAccount);
    }

    public async Task<int> ValidateUser(string username, string password)
    {
        var userAccount = await 
            GetByUserAccountByUsernameAndValidateUsername(username, ExceptionMessages.ValidateUsernameNotFound);
        ValidateAccountIsActivated(userAccount);
        return AuthenticationHelper.ValidatePassword(userAccount.PasswordSalt, userAccount.PasswordHash, password) 
            ? userAccount.Id 
            : 0;
    }

    public async Task<bool> ActivateUser(int id, Guid token)
    {
        var userAccount = await GetByUserAccountByIdAndValidateId(id);
        if (userAccount.Token != token)
        {
            return false;
        }
        await _userAccountRepository.ActivateAccount(id);
        return true;
    }

    #region Validation Methods

    private async Task<UserAccount> GetByUserAccountByUsernameAndValidateUsername(string username, string exceptionMessage)
    {
        var userAccount = await _userAccountRepository.GetByUsername(username);
        if (userAccount == null)
        {
            throw new ValidationException(string.Format(exceptionMessage, username));
        }
        return userAccount;
    }

    private async Task<UserAccount> GetByUserAccountByIdAndValidateId(int userId)
    {
        var userAccount = await _userAccountRepository.GetById(userId);
        if (userAccount == null)
        {
            throw new ValidationException(string.Format(ExceptionMessages.UserAccountUserIdNotFound));
        }
        return userAccount;
    }

    private static void ValidateAccountIsActivated(UserAccount userAccount)
    {
        if (!userAccount.IsActivated)
        {
            throw new ValidationException(string.Format(ExceptionMessages.UserAccountNotActivated,
                userAccount.EmailAddress));
        }
    }

    private async Task ValidateCreate(string userEmail, string username, string password, string passwordConfirm)
    {
        await ValidateUserAccountEmail(userEmail);
        await ValidateUserAccountUsername(username);
        ValidateUserAccountPassword(password, passwordConfirm);
    }

    private static void ValidateUserAccountPassword(string password, string passwordConfirm)
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

    private async Task ValidateUserAccountEmail(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmptyEmailAddress);
        }

        if (userEmail.Length > 256)
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmailAddressTooLong);
        }

        if (!userEmail.Contains('@'))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressNoAtSymbol,
                userEmail));
        }

        if (!userEmail.Contains('.'))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressNoDotSymbol,
                userEmail));
        }
        await ValidateDuplicateEmail(userEmail);
    }

    private async Task ValidateUserAccountUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountEmptyUsername);
        }
        if (username.Length > 30)
        {
            throw new ValidationException(ExceptionMessages.CreateUserAccountUsernameTooLong);
        }
        await ValidateDuplicateUsername(username);
    }

    private async Task ValidateDuplicateEmail(string userEmail)
    {
        if (await _userAccountRepository.ExistsWithEmail(userEmail))
        {
            throw new ValidationException(string.Format(ExceptionMessages.CreateUserAccountEmailAddressDuplicate,
                userEmail));
        }
    }

    private async Task ValidateDuplicateUsername(string username)
    {
        if (await _userAccountRepository.ExistsWithUsername(username))
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