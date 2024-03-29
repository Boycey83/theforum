using NHibernate.Criterion;
using ISession = NHibernate.ISession;
using theforum.Model;

namespace theforum.DataAccess;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly ISession _session;

    public UserAccountRepository(ISession session) => 
        _session = session;

    public async Task<int> CreateUser(UserAccount userAccount) => 
        (int) await _session.SaveAsync(userAccount);

    public async Task ChangePassword(int userAccountId, string passwordSalt, string passwordHash)
    {
        var userAccount = await _session.GetAsync<UserAccount>(userAccountId);
        userAccount.PasswordSalt = passwordSalt;
        userAccount.PasswordHash = passwordHash;
        await _session.UpdateAsync(userAccount);
    }

    public async Task<UserAccount> GetById(int id) => 
        await _session.GetAsync<UserAccount>(id);

    public async Task<UserAccount?> GetByEmail(string userAccountEmail)
    {
        UserAccount? user = null;
        var users = await _session.QueryOver(() => user)
            .Where(() => user.EmailAddress == userAccountEmail)
            .ListAsync();
        return users.SingleOrDefault();
    }

    public async Task<UserAccount?> GetByUsername(string username)
    {
        UserAccount? user = null;
        var users = await _session.QueryOver(() => user)
            .Where(() => user.Username == username)
            .ListAsync();
        return users.SingleOrDefault();
    }

    public async Task<bool> ExistsWithEmail(string userAccountEmail) =>
        await _session.QueryOver<UserAccount>()
            .Where(user => user.EmailAddress == userAccountEmail)
            .Select(Projections.RowCount())
            .SingleOrDefaultAsync<int>() > 0;

    public async Task<bool> ExistsWithUsername(string username) =>
        await _session.QueryOver<UserAccount>()
            .Where(user => user.Username == username)
            .Select(Projections.RowCount())
            .SingleOrDefaultAsync<int>() > 0;

    public async Task<bool> Exists(int userId) => 
        await _session.GetAsync<UserAccount>(userId) is not null;

    public async Task ActivateAccount(int userAccountId)
    {
        var userAccount = await _session.GetAsync<UserAccount>(userAccountId);
        userAccount.IsActivated = true;
        await _session.UpdateAsync(userAccount);
    }

    public async Task UpdateUser(UserAccount userAccount) => 
        await _session.UpdateAsync(userAccount);
}