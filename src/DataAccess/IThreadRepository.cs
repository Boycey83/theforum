using Thread=theforum.Model.Thread;

namespace theforum.DataAccess;

public interface IThreadRepository
{
    Task<int> CreateThread(Thread thread);
    Task<Thread> GetById(int threadId);
    Task UpdateLastPostDateTimeUtc(int threadId, DateTime createdDateTimeUtc);
    Task<IEnumerable<Thread>> GetTopThreads(ThreadSortOrder sortOrder, int pageNumber, int pageSize);
    Task<int> GetThreadCount();
}