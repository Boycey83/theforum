namespace theforum.ViewModels;

public record ForumViewModel(Resources.ForumResources Resources, int? ThreadId = null, int? ReplyId = null);
