using ISession = NHibernate.ISession;
using NHibernate.Linq;
using Thread = theforum.Model.Thread;

namespace theforum.DataAccess;
public class ThreadRepository : IThreadRepository
    {
        private readonly ISession _session;

        public ThreadRepository(ISession session) =>
            _session = session;

        public int CreateThread(Thread thread) =>
            (int)_session.Save(thread);

        public Thread GetById(int threadId) => 
            _session.Get<Thread>(threadId);

        public void UpdateLastPostDateTimeUtc(int threadId, DateTime lastPostDateTimeUtc)
        {
            var thread = _session.Get<Thread>(threadId);
            thread.LastPostDateTimeUtc = lastPostDateTimeUtc;
            _session.Update(thread);
        }

        public IEnumerable<Thread> GetTopThreads(ThreadSortOrder sortOrder, int pageNumber, int pageSize)
        {
            var threadsToSkip = (pageNumber - 1) * pageSize;
            var query = _session.Query<Thread>();
            query = ApplyThreadSortOrder(sortOrder, query);
            return query.Skip(threadsToSkip).Take(pageSize).Fetch(t => t.PostedBy);
        }

        public int GetThreadCount() => 
            _session.Query<Thread>().Count();

        private static IQueryable<Thread> ApplyThreadSortOrder(ThreadSortOrder sortOrder, IQueryable<Thread> query) =>
            sortOrder switch
            {
                ThreadSortOrder.Date => query.OrderBy(t => t.LastPostDateTimeUtc),
                ThreadSortOrder.DateDesc => query.OrderByDescending(t => t.LastPostDateTimeUtc),
                ThreadSortOrder.PostedBy => query.OrderBy(t => t.PostedBy.Username),
                ThreadSortOrder.PostedByDesc => query.OrderByDescending(t => t.PostedBy.Username),
                ThreadSortOrder.Subject => query.OrderBy(t => t.Title),
                ThreadSortOrder.SubjectDesc => query.OrderByDescending(t => t.Title),
                ThreadSortOrder.Size => query.OrderBy(t => t.PostCount),
                ThreadSortOrder.SizeDesc => query.OrderByDescending(t => t.PostCount),
                _ => throw new ArgumentException("ThreadSortOrder not recognised")
            };
    }