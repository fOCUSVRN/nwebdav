using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NWebDav.Sample.Kestrel.LogAdapters;
using NWebDav.Server.Logging;

namespace NWebDav.Sample.Kestrel
{
    internal class Program
    {
        private static void Main(string[] args)
        {
         // Use debug output for logging
            var adapter = new DebugOutputAdapter();
            adapter.LogLevels.Add(LogLevel.Debug);
            adapter.LogLevels.Add(LogLevel.Info);
            LoggerFactory.Factory = adapter;

            var hostingConfig = new ConfigurationBuilder()
            .AddJsonFile("hosting.json", optional: false)
            .Build();

            var host = new WebHostBuilder()
                .UseKestrel(options => { options.Limits.MaxRequestBodySize = null; })
                .UseConfiguration(hostingConfig)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}