using Microsoft.AspNetCore.Html;
using theforum.ViewModels;

namespace theforum.Views.Home;

internal static class IndexExtensions
{
    internal static string ForumId => "theforum";
    
    internal static IHtmlContent CreateKoViewModelAndBindToDom(ForumViewModel model) =>
        new HtmlString(
            $"""ko.applyBindings({GetViewModelInitializationScript(model)}, $("#{ForumId}")[0])"""
        );
    
    private static string GetViewModelInitializationScript(ForumViewModel m) => m switch
    {
        { ThreadId: not null, ReplyId: not null } =>
            $"new theforum.ForumViewModel({m.ThreadId.Value}, {m.ReplyId.Value})",
        { ThreadId: not null } => 
            $"new theforum.ForumViewModel({m.ThreadId.Value})",
        _ =>
            "new theforum.ForumViewModel()"
    };
}