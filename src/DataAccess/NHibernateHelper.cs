using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using theforum.DataAccess.Mapping;
using ISession = NHibernate.ISession;

namespace theforum.DataAccess;

public static class NHibernateHelper
{
    private static readonly object FactoryLock = new();
    private static ISessionFactory? _sessionFactory;
    private static string _connectionString = string.Empty;

    public static void Initialize(string connectionString)
    {
        _connectionString = connectionString;
    }

    private static ISessionFactory SessionFactory
    {
        get
        {
            lock (FactoryLock)
            {
                _sessionFactory ??= InitializeSessionFactory();
            }
            return _sessionFactory;
        }

    }

    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }

    private static ISessionFactory InitializeSessionFactory()
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2005.ConnectionString(_connectionString)
                .ShowSql())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserAccountMap>())
            .CurrentSessionContext<AsyncLocalSessionContext>()
            .BuildSessionFactory();
    }

    public static ISessionFactory GetSessionFactory()
    {
        return SessionFactory;
    }
}
