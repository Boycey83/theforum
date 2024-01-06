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
        _styleSettings = new StyleSettings
        {
            PrimaryBgColor = "#262729",
            PrimaryAccentColor = "#90704e",
            SecondaryAccentColor = "#ad855c",
            BodyTextColor = "#a9a9a9",
            DarkTextColor = "#2b241b",
            SectionBgColor = "#393b3d",
            DividerColor = "#46484a",
            NeutralAccentColor = "#757778",
            SoftDividerColor = "#5c5951", 
            LightAccentColor = "#b8b8b8", 
            MutedAccentColor = "#7c7875",
            DarkSectionBgColor = "#353638"
        };
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