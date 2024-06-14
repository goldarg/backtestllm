using api.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace api.Data
{
    public class RdaUnitOfWork : IRdaUnitOfWork
    {
        private readonly RdaDbContext _dbContext;
        private readonly IRepositoryFactory _repositoryFactory;
        private Dictionary<string, object> _repositories;

        public RdaDbContext DbContext => _dbContext;

        public RdaUnitOfWork(RdaDbContext dbContext, IRepositoryFactory repositoryFactory)
        {
            _dbContext = dbContext;
            _repositoryFactory = repositoryFactory;
            _repositories = new Dictionary<string, object>();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private bool disposed = false;

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories.TryGetValue(typeof(TEntity).FullName!, out var repository))
            {
                if (repository is Repository<TEntity> repositoryCasted)
                    return repositoryCasted;
                throw new ArgumentException("No se pudo recuperar el repositorio");
            }

            var repositoryToReturn = _repositoryFactory.CreateRepository<TEntity>();
            _repositories.Add(typeof(TEntity).FullName!, repositoryToReturn);

            return repositoryToReturn;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}