using theforum.Model;

namespace theforum.DataAccess;

public interface IPostRepository
{
    Post GetById(int value);
    int CreatePost(Post reply);
    IEnumerable<Post> GetTopPostsByThreadId(int threadId);
}