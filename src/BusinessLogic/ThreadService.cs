using System.ComponentModel.DataAnnotations;
using theforum.DataAccess;
using theforum.Model;
using theforum.Resources;
using Thread = theforum.Model.Thread;

namespace theforum.BusinessLogic;

public class ThreadService
{
    private readonly IThreadRepository _threadRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public ThreadService(IUserAccountRepository userAccountRepository, IThreadRepository threadRepository)
    {
        _userAccountRepository = userAccountRepository;
        _threadRepository = threadRepository;
    }

    public async Task<Thread> CreateThread(int userId, string title, string message)
    {
        title = title.Trim();
        message = message.Trim();
        var postedBy = await _userAccountRepository.GetById(userId);
        ValidateCreateThread(postedBy, title, message);
        var createdDateTimeUtc = DateTime.UtcNow;
        var thread = new Thread(postedBy, title, message, createdDateTimeUtc);
        var threadId = await _threadRepository.CreateThread(thread);
        thread.Id = threadId;
        return thread;
    }

    public async Task<IEnumerable<Thread>> GetTopThreads(ThreadSortOrder sortOrder, int pageNumber, int pageSize) => 
        await _threadRepository.GetTopThreads(sortOrder, pageNumber, pageSize);

    public async Task<int> GetThreadCount() =>
        await _threadRepository.GetThreadCount();

    private static void ValidateCreateThread(UserAccount postedBy, string title, string message)
    {
        if (postedBy == null)
        {
            throw new ValidationException(ExceptionMessages.CreateThreadUserAccountNotFound);
        }
        if (title == null || title.Length < 3 || title.Length > 200)
        {
            throw new ValidationException(ExceptionMessages.CreateThreadTitleError);
        }
        if (message == null || message.Length < 3 || message.Length > 4000)
        {
            throw new ValidationException(ExceptionMessages.CreateThreadMessageError);
        }
    }
}