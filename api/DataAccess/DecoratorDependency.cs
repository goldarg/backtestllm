namespace api.DataAccess;

public abstract class DecoratorDependency<TService>
    where TService : class, IDecoratorDependency<TService>
{
    public TService InnerService { get; }

    protected DecoratorDependency(TService innerDecoratedService)
    {
        InnerService =
            GetType().BaseType?.Name == typeof(DecoratorDependency<TService>).Name
                ? innerDecoratedService
                : innerDecoratedService.InnerService;
    }
}
