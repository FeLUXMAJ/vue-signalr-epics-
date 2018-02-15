using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LesulaSPA.Hubs.Services
{
    public class ClockService : BackgroundService
    {
        private Timer _timer;
        private readonly IHubContext<CAMonitorHub> _context;

        public ClockService(IHubContext<CAMonitorHub> context)
        {
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(FastTick, this, 0, 100);
        }

        protected new async Task StopAsync(CancellationToken stoppingToken)
        {
            // Run your graceful clean-up actions
            _timer.Dispose();
        }

        private void FastTick(object state)
        {
            _context.Clients.All.InvokeAsync("fastTick", DateTime.Now);
        }
    }
}
