using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Data
{
    public interface IDecoratorDependency<out TService> where TService : class
    {
        TService InnerService { get; }
    }
}