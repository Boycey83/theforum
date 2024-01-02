using NHibernate;
using ISession = NHibernate.ISession;

namespace theforum.DataAccess;

public abstract class RepositoryBase : IRepository
{
    private readonly ISession _session;
    private ITransaction? _transaction;

    protected RepositoryBase()
    {
    }

    protected RepositoryBase(ISession session)
    {
        _session = session;
    }

    public void BeginTransaction()
    {
        _transaction = _session.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
    }

    public void DisposeTransaction()
    {
        _transaction?.Dispose();
    }
}
