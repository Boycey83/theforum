using Thread=theforum.Model.Thread;

namespace theforum.DataAccess;

public interface IThreadRepository
{
    int CreateThread(Thread thread);
    Thread GetById(int threadId);
    void UpdateLastPostDateTimeUtc(int threadId, DateTime createdDateTimeUtc);
    IEnumerable<Thread> GetTopThreads(ThreadSortOrder sortOrder, int pageNumber, int pageSize);
    int GetThreadCount();
}