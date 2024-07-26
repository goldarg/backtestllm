namespace api.DataAccess;

public interface IDecoratorDependency<out TService>
    where TService : class
{
    TService InnerService { get; }
}
