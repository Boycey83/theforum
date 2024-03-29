using theforum.Model;

namespace theforum.DataAccess;

public interface IPostRepository
{
    Task<Post> GetById(int value);
    Task<int> CreatePost(Post reply);
    Task<IEnumerable<Post>> GetTopPostsByThreadId(int threadId);
}