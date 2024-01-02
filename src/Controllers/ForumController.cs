using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using theforum.BusinessLogic;
using theforum.DataAccess;
using theforum.Dto;
using theforum.Resources;

namespace theforum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumController : Controller
{
    private const int PageSize = 25;
    private readonly PostService _postService;
    private readonly ThreadService _threadService;

    public ForumController(PostService postService, ThreadService threadService)
    {
        _postService = postService;
        _threadService = threadService;
    }

    #region Public Actions

    [HttpGet]
    [Route("Threads")]
    public ThreadsContextDto GetThreads()
    {
        return GetThreads(ThreadSortOrder.DateDesc, 1);
    }

    [HttpGet]
    [Route("Threads/Page/{pageNumber:int}")]
    public ThreadsContextDto GetThreads(int pageNumber)
    {
        return GetThreads(ThreadSortOrder.DateDesc, pageNumber);
    }

    [HttpGet]
    [Route("Threads/Sort/{sortOrder}")]
    public ThreadsContextDto GetThreads(ThreadSortOrder sortOrder)
    {
        return GetThreads(sortOrder, 1);
    }

    [HttpGet]
    [Route("Threads/Sort/{sortOrder}/Page/{pageNumber:int}")]
    public ThreadsContextDto GetThreads(ThreadSortOrder sortOrder, int pageNumber)
    {
        var threads = _threadService.GetTopThreads(sortOrder, pageNumber, PageSize);
        var threadCount = _threadService.GetThreadCount();
        return new ThreadsContextDto(
            Threads: threads.Select(ThreadDto.BuildFromThread),
            ThreadCount: threadCount,
            LoggedInAsUsername: User.Identity?.Name ?? string.Empty,
            PageNumber: pageNumber
        );
    }

    [HttpGet]
    [Route("Thread/{threadId:int}/Replies")]
    public IEnumerable<PostDto> GetThreadReplies(int threadId)
    {
        var posts = _postService.GetThreadReplies(threadId);
        return posts.Select(PostDto.BuildFromPost);
    }

    #endregion

    #region Logged In Actions

    [HttpPost]
    [Route("Thread")]
    public ThreadDto CreateNewThread(CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreateThreadMustBeLoggedIn);
        }
        var thread = _threadService.CreateThread(userId, postDto.Title, postDto.Message);
        return ThreadDto.BuildFromThread(thread);
    }

    [HttpPost]
    [Route("Thread/{threadId:int}/Reply")]
    public PostDto CreateReplyToThread(int threadId, CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreatePostMustBeLoggedIn);
        }
        var post = _postService.CreatePost(threadId, null, userId, postDto.Title, postDto.Message);
        return PostDto.BuildFromPost(post);
    }

    [HttpPost]
    [Route("Thread/{threadId:int}/Post/{postId:int}/Reply")]
    public PostDto CreateReplyToPost(int threadId, int postId, CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreatePostMustBeLoggedIn);
        }
        var post = _postService.CreatePost(threadId, postId, userId, postDto.Title, postDto.Message);
        return PostDto.BuildFromPost(post);
    }

    #endregion
}