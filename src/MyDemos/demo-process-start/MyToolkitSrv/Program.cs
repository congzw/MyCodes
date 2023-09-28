using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MyToolkitSrv
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(cfg =>
            {
                cfg.AddHostedService<MyHostedService>();
            });

            using IHost host = builder.Build();
            await host.RunAsync();
            DemoService.Log("## Main Exit!!!");
        }
    }
}