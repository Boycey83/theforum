using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using theforum.BusinessLogic;
using theforum.Resources;
using theforum.Settings;
using theforum.ViewModels;

namespace theforum.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly UserAccountService _userAccountService;
    private readonly ForumResources _forumResources;
    private readonly StyleSettings _styleSettings;

    public HomeController(UserAccountService userAccountService, IOptions<ForumResources> forumResources)
    {
        _userAccountService = userAccountService;
        _forumResources = forumResources.Value;
        _styleSettings = new StyleSettings { BodyBgColor = "#262729" };
    }

    [HttpGet]
    public IActionResult Index() => 
        View(new ForumViewModel(_forumResources, _styleSettings));

    [HttpGet("Thread/{threadId:int}")]
    public IActionResult Thread(int threadId) => 
        View("Index", new ForumViewModel(_forumResources, _styleSettings, threadId));

    [HttpGet("Thread/{threadId:int}/Reply/{replyId:int}")]
    public IActionResult Reply(int threadId, int replyId) => 
        View("Index", new ForumViewModel(_forumResources, _styleSettings, threadId, replyId));

    [HttpGet("UserAccount/{userAccountId:int}/Confirm/{tokenString}")]
    public IActionResult ConfirmUserAccount(int userAccountId, string tokenString)
    {
        var token = Guid.Parse(tokenString);
        _userAccountService.ActivateUser(userAccountId, token);
        return RedirectToAction("Index");
    }
}