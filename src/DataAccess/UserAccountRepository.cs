using ISession = NHibernate.ISession;
using theforum.Model;

namespace theforum.DataAccess;

public class UserAccountRepository : RepositoryBase, IUserAccountRepository
{
    private readonly ISession _session;

    public UserAccountRepository(ISession session)
    {
        _session = session;
    }

    public int CreateUser(UserAccount userAccount)
    {
        return (int)_session.Save(userAccount);
    }

    public void ChangePassword(int userAccountId, string passwordSalt, string passwordHash)
    {
        var userAccount = _session.Get<UserAccount>(userAccountId);
        userAccount.PasswordSalt = passwordSalt;
        userAccount.PasswordHash = passwordHash;
        _session.Update(userAccount);
    }

    public UserAccount GetById(int id)
    {
        return _session.Get<UserAccount>(id);
    }

    public UserAccount? GetByEmail(string userAccountEmail)
    {
        return _session.Query<UserAccount>().SingleOrDefault(u => u.EmailAddress == userAccountEmail);
    }

    public UserAccount? GetByUsername(string username)
    {
        return _session.Query<UserAccount>().SingleOrDefault(u => u.Username == username);
    }

    public bool ExistsWithEmail(string userAccountEmail)
    {
        return _session.Query<UserAccount>().Any(u => u.EmailAddress == userAccountEmail);
    }

    public bool ExistsWithUsername(string username)
    {
        return _session.Query<UserAccount>().Any(u => u.Username == username);
    }

    public bool Exists(int userId)
    {
        return _session.Query<UserAccount>().Any(u => u.Id == userId);
    }

    public void ActivateAccount(int userAccountId)
    {
        var userAccount = _session.Get<UserAccount>(userAccountId);
        userAccount.IsActivated = true;
        _session.Update(userAccount);
    }

    public void UpdateUser(UserAccount userAccount)
    {
        _session.Update(userAccount);
    }
}