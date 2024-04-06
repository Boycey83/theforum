using Microsoft.AspNetCore.Mvc;
using theforum.Whitelabel;

namespace theforum.Controllers;

[Route("favicon")]
public class FaviconController : Controller
{
    private readonly FaviconService _faviconService;

    public FaviconController(FaviconService faviconService) => 
        _faviconService = faviconService;

    [HttpGet("{size}.png")]
    public IActionResult GetFavicon(string size)
    {
        Response.Headers["Cache-Control"] = "public,max-age=3600"; // Cache for 1 hour
        var favicon = _faviconService.GetFavicon(size);
        return favicon is not null 
            ? File(favicon, "image/png") 
            : NotFound();
    }
}
