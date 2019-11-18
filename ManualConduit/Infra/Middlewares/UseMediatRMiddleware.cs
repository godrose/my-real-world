using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit.Infra.Middlewares
{
    public class UseMediatRMiddleware : IMiddleware<IServiceCollection>
    {
        private readonly Assembly _assembly;

        public UseMediatRMiddleware(Assembly assembly)
        {
            _assembly = assembly;
        }

        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddMediatR(_assembly);
            @object.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            @object.AddScoped(typeof(IPipelineBehavior<,>), typeof(DBContextTransactionPipelineBehavior<,>));
            return @object;
        }
    }
}
