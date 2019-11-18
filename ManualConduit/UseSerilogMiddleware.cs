using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Solid.Practices.Middleware;

namespace ManualConduit
{
    public class UseSerilogMiddleware : IMiddleware<ILoggerFactory>
    {
        public ILoggerFactory Apply(ILoggerFactory @object)
        {
            // Attach the sink to the logger configuration
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                //just for local debug
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            @object.AddSerilog(log);
            Log.Logger = log;
            return @object;
        }
    }
}