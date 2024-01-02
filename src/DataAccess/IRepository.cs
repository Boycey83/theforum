namespace theforum.DataAccess;

public interface IRepository
{
void BeginTransaction();
void CommitTransaction();
void DisposeTransaction();
}