using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwsEntity;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AwsWorker
{
    public class Worker : BackgroundService
    {
        readonly IBus _bus;

        public Worker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _bus.Publish(new MessageTest { Text = $"The time is {DateTimeOffset.Now}" });
                }
                catch (Exception ex)
                {
                   Console.WriteLine(ex.ToString());
                }
                

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
