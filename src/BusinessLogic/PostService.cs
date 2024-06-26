using System.ComponentModel.DataAnnotations;
using theforum.DataAccess;
using theforum.Model;
using theforum.Resources;
using Thread=theforum.Model.Thread;

namespace theforum.BusinessLogic;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IThreadRepository _threadRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public PostService(IUserAccountRepository userAccountRepository, IPostRepository postRepository, IThreadRepository threadRepository)
    {
        _userAccountRepository = userAccountRepository;
        _postRepository = postRepository;
        _threadRepository = threadRepository;
    }

    public async Task<Post> CreatePost(int threadId, int? parentPostId, int userId, string title, string message)
    {
        title = title.Trim();
        message = message.Trim();
        var parent = parentPostId.HasValue ? await _postRepository.GetById(parentPostId.Value) : null;
        var thread = await _threadRepository.GetById(threadId);
        var postedBy = await _userAccountRepository.GetById(userId);
        if (parent != null)
        {
            ValidateCreatePost(postedBy, thread, parent, title, message);
        }
        else
        {
            ValidateCreatePost(postedBy, thread, title, message);
        }
        var createdDateTimeUtc = DateTime.UtcNow;
        var post = new Post(thread, parent, postedBy, title, message, createdDateTimeUtc);
        var postId = await _postRepository.CreatePost(post);
        await _threadRepository.UpdateLastPostDateTimeUtc(threadId, createdDateTimeUtc);
        post.Id = postId;
        return post;
    }

    public async Task<IEnumerable<Post>> GetThreadReplies(int threadId) => 
        await _postRepository.GetTopPostsByThreadId(threadId);

    #region Validation Methods

    private static void ValidateCreatePost(UserAccount postedBy, Thread thread, string title, string message)
    {
        if (postedBy == null)
        {
            throw new ValidationException(ExceptionMessages.CreatePostUserAccountNotFound);
        }
        if (thread == null)
        {
            throw new ValidationException(ExceptionMessages.CreatePostThreadNotFound);
        }
        if (title == null || title.Length < 3 || title.Length > 200)
        {
            throw new ValidationException(ExceptionMessages.CreatePostTitleError);
        }
        if (message == null ||  message.Length < 3 || message.Length > 4000)
        {
            throw new ValidationException(ExceptionMessages.CreatePostMessageError);
        }
    }

    private static void ValidateCreatePost(UserAccount postedBy, Thread thread, Post parent, string title, string message)
    {
        if (parent == null)
        {
            throw new ValidationException(ExceptionMessages.CreatePostParentPostNotFound);
        }
        ValidateCreatePost(postedBy, thread, title, message);
    }

    #endregion
}