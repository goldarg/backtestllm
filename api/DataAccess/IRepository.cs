using System.Linq.Expressions;

namespace api.DataAccess;

public interface IRepository<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetRepository();

    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = ""
    );

    public TEntity? GetById(object id);

    public void Insert(TEntity entity);

    public void Delete(object id);

    public void Delete(TEntity entityToDelete);

    public void SaveChanges();

    public void Update(TEntity entityToUpdate);
}
