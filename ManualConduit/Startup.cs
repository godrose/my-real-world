using System.Reflection;
using ManualConduit.Infra;
using ManualConduit.Infra.Errors;
using ManualConduit.Infra.Middlewares;
using Microsoft.AspNetCore.Builder;
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
                new UseAutoMapperMiddleware(),
                new UseJwtMiddleware(),
                new UseOtherMiddleware()
            };
            MiddlewareApplier.ApplyMiddlewares(services, middlewares);

            return services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var loggerFactoryMiddlewares = new IMiddleware<ILoggerFactory>[]
            {
                new UseSerilogMiddleware()
            };
            MiddlewareApplier.ApplyMiddlewares(loggerFactory, loggerFactoryMiddlewares);
            app.UseMiddleware<ErrorHandlingMiddleware>()
                .UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseMvc()
                // Enable middleware to serve generated Swagger as a JSON endpoint
                .UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; })
                // Enable middleware to serve swagger-ui assets(HTML, JS, CSS etc.)
                .UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "RealWorld API V1"); });

            app.ApplicationServices.GetRequiredService<ConduitContext>().Database.EnsureCreated();
        }
    }
}
