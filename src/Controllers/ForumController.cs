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
    public async Task<ThreadsContextDto> GetThreads() => 
        await GetThreads(ThreadSortOrder.DateDesc, 1);

    [HttpGet]
    [Route("Threads/Page/{pageNumber:int}")]
    public async Task<ThreadsContextDto> GetThreads(int pageNumber) => 
        await GetThreads(ThreadSortOrder.DateDesc, pageNumber);

    [HttpGet]
    [Route("Threads/Sort/{sortOrder}")]
    public async Task<ThreadsContextDto> GetThreads(ThreadSortOrder sortOrder) => 
        await GetThreads(sortOrder, 1);

    [HttpGet]
    [Route("Threads/Sort/{sortOrder}/Page/{pageNumber:int}")]
    public async Task<ThreadsContextDto> GetThreads(ThreadSortOrder sortOrder, int pageNumber)
    {
        var threads = await _threadService.GetTopThreads(sortOrder, pageNumber, PageSize);
        var threadCount = await _threadService.GetThreadCount();
        return new ThreadsContextDto(
            Threads: threads.Select(ThreadDto.BuildFromThread),
            ThreadCount: threadCount,
            LoggedInAsUsername: User.Identity?.Name ?? string.Empty,
            PageNumber: pageNumber
        );
    }

    [HttpGet]
    [Route("Thread/{threadId:int}/Replies")]
    public async Task<IEnumerable<PostDto>> GetThreadReplies(int threadId)
    {
        var posts = await _postService.GetThreadReplies(threadId);
        return posts.Select(PostDto.BuildFromPost);
    }

    #endregion

    #region Logged In Actions

    [HttpPost]
    [Route("Thread")]
    public async Task<ThreadDto> CreateNewThread(CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreateThreadMustBeLoggedIn);
        }
        var thread = await _threadService.CreateThread(userId, postDto.Title, postDto.Message);
        return ThreadDto.BuildFromThread(thread);
    }

    [HttpPost]
    [Route("Thread/{threadId:int}/Reply")]
    public async Task<PostDto> CreateReplyToThread(int threadId, CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreatePostMustBeLoggedIn);
        }
        var post = await _postService.CreatePost(threadId, null, userId, postDto.Title, postDto.Message);
        return PostDto.BuildFromPost(post);
    }

    [HttpPost]
    [Route("Thread/{threadId:int}/Post/{postId:int}/Reply")]
    public async Task<PostDto> CreateReplyToPost(int threadId, int postId, CreatePostDto postDto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new ValidationException(ExceptionMessages.CreatePostMustBeLoggedIn);
        }
        var post = await _postService.CreatePost(threadId, postId, userId, postDto.Title, postDto.Message);
        return PostDto.BuildFromPost(post);
    }
    
    #endregion
}