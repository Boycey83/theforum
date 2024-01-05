using theforum.Settings;

namespace theforum.ViewModels;

public record ForumViewModel(
    Resources.ForumResources Resources, 
    StyleSettings StyleSettings,
    int? ThreadId = null, 
    int? ReplyId = null);
