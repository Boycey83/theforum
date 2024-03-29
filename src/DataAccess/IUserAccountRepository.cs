using theforum.Model;

namespace theforum.DataAccess;

public interface IUserAccountRepository
{
    Task<int> CreateUser(UserAccount userAccount);
    Task ChangePassword(int userAccountId, string passwordSalt, string passwordHash);
    Task<UserAccount?> GetByEmail(string userEmail);
    Task<UserAccount?> GetByUsername(string username);
    Task<bool> ExistsWithEmail(string userEmail);
    Task<bool> ExistsWithUsername(string userEmail);
    Task<bool> Exists(int userId);
    Task<UserAccount> GetById(int userId);
    Task ActivateAccount(int id);
    Task UpdateUser(UserAccount userAccount);
}