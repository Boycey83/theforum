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
    public static ThreadDto BuildFromThread(Thread t) =>
        new(
            Id: t.Id,
            Title: t.Title,
            Message: t.Message,
            PostedByUsername: t.PostedBy.Username,
            PostedByEmailAddress: t.PostedBy.EmailAddress,
            CreatedDateTimeUtc: t.CreatedDateTimeUtc,
            LastPostDateTimeUtc: t.LastPostDateTimeUtc,
            PostCount: t.PostCount + 1
        );
}
