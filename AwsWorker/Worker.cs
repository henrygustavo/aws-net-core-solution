using System;
using System.Threading;
using System.Threading.Tasks;
using AwsDomain;
using MassTransit;
using Microsoft.Extensions.Hosting;

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
