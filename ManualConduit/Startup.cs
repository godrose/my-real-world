using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddServicesImpl(services);
        }

        private IServiceCollection AddServicesImpl(IServiceCollection services)
        {
            var middlewares = new IMiddleware<IServiceCollection>[]
            {
                new UseMediatRMiddleware(Assembly.GetExecutingAssembly()), 
                new UseDbContextMiddleware(_config),
                new UseLocalizationMiddleware(),
                new UseSwaggerMiddleware(),
                new UseCorsMiddleware(), 
                new UseMvcMiddleware(),
                new UseAutoMapperMiddleware()
            };
            MiddlewareApplier.ApplyMiddlewares(services, middlewares);
            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
