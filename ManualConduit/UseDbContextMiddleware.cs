using System;
using ManualConduit.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseDbContextMiddleware : IMiddleware<IServiceCollection>
    {
        public const string DEFAULT_DATABASE_CONNECTIONSTRING = "Filename=realworld.db";
        public const string DEFAULT_DATABASE_PROVIDER = "sqlite";

        private readonly IConfiguration _config;

        public UseDbContextMiddleware(IConfiguration config)
        {
            _config = config;
        }

        public IServiceCollection Apply(IServiceCollection @object)
        {
            var connectionString = _config.GetValue<string>("ASPNETCORE_Conduit_ConnectionString") ??
                                   DEFAULT_DATABASE_CONNECTIONSTRING;
            // take the database provider from the environment variable or use hard-coded database provider
            var databaseProvider = _config.GetValue<string>("ASPNETCORE_Conduit_DatabaseProvider");
            if (string.IsNullOrWhiteSpace(databaseProvider))
                databaseProvider = DEFAULT_DATABASE_PROVIDER;

            @object.AddDbContext<ConduitContext>(options =>
            {
                if (databaseProvider.ToLower().Trim().Equals("sqlite"))
                    options.UseSqlite(connectionString);
                else if (databaseProvider.ToLower().Trim().Equals("sqlserver"))
                {
                    // only works in windows container
                    options.UseSqlServer(connectionString);
                }
                else
                    throw new Exception("Database provider unknown. Please check configuration");
            });
            return @object;
        }
    }
}
