using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseAutoMapperMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddAutoMapper(GetType().Assembly);
            return @object;
        }
    }
}
