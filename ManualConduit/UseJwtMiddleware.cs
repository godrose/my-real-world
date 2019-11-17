using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseJwtMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            @object.AddJwt();
            return @object;
        }
    }
}
