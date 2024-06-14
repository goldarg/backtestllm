using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Data
{
    public interface IRdaUnitOfWork
    {
        void SaveChanges();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void Dispose();

        RdaDbContext DbContext { get; }
    }
}