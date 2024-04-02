using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using theforum.Model;
using ISession = NHibernate.ISession;

namespace theforum.DataAccess;

public class PostRepository : IPostRepository
{
    private readonly ISession _session;

    public PostRepository(ISession session) =>
        _session = session;

    public async Task<Post> GetById(int id) => 
        await _session.GetAsync<Post>(id);

    public async Task<int> CreatePost(Post reply) => 
        (int) await _session.SaveAsync(reply);

    public async Task<IEnumerable<Post>> GetTopPostsByThreadId(int threadId)
    {
        var posts = await _session
            .CreateCriteria<Post>()
            .Add(Restrictions.Eq("Thread.Id", threadId))
            .AddOrder(Order.Asc("CreatedDateTimeUtc"))
            .SetFetchMode("Replies", FetchMode.Join)
            .CreateAlias("Replies", "r", JoinType.LeftOuterJoin)
            .AddOrder(Order.Asc("r.CreatedDateTimeUtc"))
            .SetFetchMode("PostedBy", FetchMode.Join)
            .SetResultTransformer(new DistinctRootEntityResultTransformer())
            .SetMaxResults(1250)
            .ListAsync<Post>();
        return posts.Where(p => p.Parent == null);
    }
}
