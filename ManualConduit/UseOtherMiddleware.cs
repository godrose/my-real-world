using System;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseOtherMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddScoped<IPasswordHasher, PasswordHasher>();
            @object.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            @object.AddScoped<IProfileReader, ProfileReader>();
            @object.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
