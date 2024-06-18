namespace api.DataAccess;

public interface IRdaUnitOfWork
{
    void SaveChanges();

    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    void Dispose();

    RdaDbContext DbContext { get; }
}