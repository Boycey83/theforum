using theforum.Model;

namespace theforum.Dto;

public record PostDto(
    int Id,
    string Title,
    string Message,
    string PostedByUsername,
    string PostedByEmailAddress,
    DateTime CreatedDateTimeUtc,
    IEnumerable<PostDto> Replies,
    int ThreadId
)
{
    public static PostDto BuildFromPost(Post post) =>
        new (
            Id: post.Id,
            Title: post.Title,
            Message: post.Message,
            PostedByUsername: post.PostedBy.Username,
            PostedByEmailAddress: post.PostedBy.EmailAddress,
            CreatedDateTimeUtc: post.CreatedDateTimeUtc,
            Replies: post.Replies.Select(BuildFromPost),
            ThreadId: post.Thread.Id
        );
}
