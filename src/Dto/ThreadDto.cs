using Thread = theforum.Model.Thread;

namespace theforum.Dto;

public record ThreadDto(
    int Id,
    string Title,
    string Message,
    string PostedByUsername,
    string PostedByEmailAddress,
    DateTime CreatedDateTimeUtc,
    DateTime LastPostDateTimeUtc,
    int PostCount
)
{
    public static ThreadDto BuildFromThread(Thread thread) =>
        new(
            Id: thread.Id,
            Title: thread.Title,
            Message: thread.Message,
            PostedByUsername: thread.PostedBy.Username,
            PostedByEmailAddress: thread.PostedBy.EmailAddress,
            CreatedDateTimeUtc: thread.CreatedDateTimeUtc,
            LastPostDateTimeUtc: thread.LastPostDateTimeUtc,
            PostCount: thread.PostCount + 1
        );
}
