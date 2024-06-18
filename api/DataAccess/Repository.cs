using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace api.DataAccess;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(IRdaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _dbSet = unitOfWork.DbContext.Set<TEntity>();
    }

    public IQueryable<TEntity> GetRepository()
    {
        return _dbSet.AsNoTracking();
    }

    public virtual IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (filter != null) query = query.Where(filter);

        foreach (var includeProperty in includeProperties.Split
                     (new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            query = query.Include(includeProperty);

        if (orderBy != null)
            return orderBy(query);
        else
            return query;
    }

    public virtual TEntity? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public virtual void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
        var entityToDelete = _dbSet.Find(id);
        if (entityToDelete != null) Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (_dbSet.Entry(entityToDelete).State == EntityState.Detached) _dbSet.Attach(entityToDelete);
        _dbSet.Remove(entityToDelete);
    }

    public void SaveChanges()
    {
        _unitOfWork.SaveChanges();
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        _dbSet.Entry(entityToUpdate).State = EntityState.Modified;
    }
}