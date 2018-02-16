using EPICSsharp.CA.Client;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace vueSignalREpicsSharp.Hubs.Services
{
    public class CAMonitorService : BackgroundService
    {
        private readonly IHubContext<CAMonitorHub> _context;
        private string _pv = "sim:count";
        private CAClient _ca_client = new CAClient();
        private Dictionary<string, Channel<string>> _monitoredChannels = new Dictionary<string, Channel<string>>();
        private bool _startedCommunication = false;
        private string _gateway = null;

        public string Gateway
        {
            get
            {
                return _gateway;
            }
            set
            {
                if (_startedCommunication)
                {
                    throw new NotSupportedException("You cannot change the gateway after communications started. Set the Gateway first!");
                }
                if (value == null)
                    return;
                // This is the programmatic way to set up a Gateway for
                // PV searches. An alternative way would be to modify
                // App.config and set it there, e.g.
                // <appSettings>
                //   <add key="e#ServerList" value="192.168.1.50"/>
                // </appSettings>
                _ca_client.Configuration.SearchAddress = value;
            }
        }

        public CAMonitorService(IHubContext<CAMonitorHub> context)
        {
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Gateway = "130.246.71.15";

            if (_monitoredChannels.ContainsKey(_pv))
            {
                Console.WriteLine("PV {0} is already being monitored!", _pv);
            }
            else
            {
                try
                {
                    Channel<string> channel = _ca_client.CreateChannel<string>(_pv);
                    channel.MonitorChanged += Channel_MonitorChanged;
                    _monitoredChannels[_pv] = channel;
                    _startedCommunication = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _startedCommunication = false;
                }
            }
        }

        protected new async Task StopAsync(CancellationToken stoppingToken)
        {
            // Graceful clean-up actions
            _ca_client.Dispose();
        }

        private void Channel_MonitorChanged(Channel<string> sender, string newValue)
        {
            //Console.WriteLine("{0}: {1}", sender.ChannelName, newValue);
            var changedPV = new ProcessVariable
            {
                Name = sender.ChannelName,
                Value = newValue
            };
            _context.Clients.All.InvokeAsync("minitoredPV", changedPV);
        }
    }

    public class ProcessVariable
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
