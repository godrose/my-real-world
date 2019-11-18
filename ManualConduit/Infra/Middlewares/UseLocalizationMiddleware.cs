using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit.Infra.Middlewares
{
    public class UseLocalizationMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddLocalization(x => x.ResourcesPath = "Resources");
            return @object;
        }
    }
}
