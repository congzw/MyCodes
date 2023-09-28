using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Common;
using System;

namespace MyToolkitSrv
{
    public sealed class MyHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime appLife;

        public MyHostedService(ILogger<MyHostedService> logger, IHostApplicationLifetime appLife)
        {
            _logger = logger;
            this.appLife = appLife;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            DemoService.Log("## StartAsync Invoked");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            DemoService.Log("## StopAsync Invoked");
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                DemoService.Run();
                DemoService.Log("## ExecuteAsync Invoked");
            }
            catch (Exception ex)
            {
                DemoService.Log("## ExecuteAsync Ex " + ex.Message);
            }
            finally
            {
                appLife.StopApplication();
                DemoService.Log("## StopApplication Invoked");
            }
            return Task.CompletedTask;
        }
    }
}