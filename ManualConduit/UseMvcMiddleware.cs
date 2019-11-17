using FluentValidation.AspNetCore;
using ManualConduit.Infra;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseMvcMiddleware : IMiddleware<IServiceCollection>
    {
        public IServiceCollection Apply(IServiceCollection @object)
        {
            @object.AddMvc(opt =>
                {
                    opt.Conventions.Add(new GroupByApiRootConvention());
                    opt.Filters.Add(typeof(ValidatorActionFilter));
                    opt.EnableEndpointRouting = false;
                })
                .AddJsonOptions(opt => { opt.JsonSerializerOptions.IgnoreNullValues = true; })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });
            return @object;
        }
    }
}
