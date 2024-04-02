using NHibernate;
using NHibernate.Criterion;
using ISession = NHibernate.ISession;
using NHibernate.SqlCommand;
using theforum.Model;
using Thread = theforum.Model.Thread;

namespace theforum.DataAccess;

public class ThreadRepository : IThreadRepository
{
    private readonly ISession _session;

    public ThreadRepository(ISession session) =>
        _session = session;

    public async Task<int> CreateThread(Thread thread) =>
        (int)await _session.SaveAsync(thread);

    public async Task<Thread> GetById(int threadId) =>
        await _session.GetAsync<Thread>(threadId);

    public async Task UpdateLastPostDateTimeUtc(int threadId, DateTime lastPostDateTimeUtc)
    {
        var thread = _session.Get<Thread>(threadId);
        thread.LastPostDateTimeUtc = lastPostDateTimeUtc;
        await _session.UpdateAsync(thread);
    }

    public async Task<IEnumerable<Thread>> GetTopThreads(ThreadSortOrder sortOrder, int pageNumber, int pageSize)
    {
        var threadsToSkip = (pageNumber - 1) * pageSize;
        var queryOver = _session.QueryOver<Thread>();
        queryOver = ApplyThreadSortOrder(sortOrder, queryOver);
        queryOver.Fetch(SelectMode.Fetch, x => x.PostedBy);
        return await queryOver.Skip(threadsToSkip).Take(pageSize).ListAsync<Thread>();
    }

    public async Task<int> GetThreadCount() =>
        await _session.QueryOver<Thread>()
            .ToRowCountQuery()
            .RowCountAsync();
    
    private static IQueryOver<Thread, Thread> ApplyThreadSortOrder(ThreadSortOrder sortOrder,
        IQueryOver<Thread, Thread> query)
    {
        UserAccount userAlias = null;
        query.JoinAlias(t => t.PostedBy, () => userAlias, JoinType.LeftOuterJoin);
        switch (sortOrder)
        {
            case ThreadSortOrder.Date:
                query.OrderBy(x => x.LastPostDateTimeUtc).Asc();
                break;
            case ThreadSortOrder.DateDesc:
                query.OrderBy(x => x.LastPostDateTimeUtc).Desc();
                break;
            case ThreadSortOrder.PostedBy:
                query.OrderBy(() => userAlias.Username).Asc();
                break;
            case ThreadSortOrder.PostedByDesc:
                query.OrderBy(() => userAlias.Username).Desc();
                break;
            case ThreadSortOrder.Subject:
                query.OrderBy(x => x.Title).Asc();
                break;
            case ThreadSortOrder.SubjectDesc:
                query.OrderBy(x => x.Title).Desc();
                break;
            case ThreadSortOrder.Size:
                query.OrderBy(x => x.PostCount).Asc();
                break;
            case ThreadSortOrder.SizeDesc:
                query.OrderBy(x => x.PostCount).Desc();
                break;
            default:
                throw new ArgumentException("ThreadSortOrder not recognised");
        }
        return query;
    }
}