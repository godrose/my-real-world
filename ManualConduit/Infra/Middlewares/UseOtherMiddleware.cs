using ManualConduit.Infra.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit.Infra.Middlewares
{
    public class UseOtherMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddScoped<IPasswordHasher, PasswordHasher>();
            @object.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            /* TODO: Move to Module
            @object.AddScoped<IProfileReader, ProfileReader>();
            */
            @object.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return @object;
        }
    }
}
