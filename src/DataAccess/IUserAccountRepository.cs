using theforum.Model;

namespace theforum.DataAccess;

public interface IUserAccountRepository
{
    int CreateUser(UserAccount userAccount);
    void ChangePassword(int userAccountId, string passwordSalt, string passwordHash);
    UserAccount? GetByEmail(string userEmail);
    UserAccount? GetByUsername(string username);
    bool ExistsWithEmail(string userEmail);
    bool ExistsWithUsername(string userEmail);
    bool Exists(int userId);
    UserAccount GetById(int userId);
    void ActivateAccount(int id);
    void UpdateUser(UserAccount userAccount);
}