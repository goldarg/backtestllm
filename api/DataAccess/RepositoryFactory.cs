namespace api.DataAccess;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRepository<TEntity> CreateRepository<TEntity>()
        where TEntity : class
    {
        return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
    }
}
