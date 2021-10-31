using AwsDomain;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AwsReceiver.Consumers
{
    public class UserEventConsumer : IConsumer<UserEvent>
    {
        readonly ILogger<UserEventConsumer> _logger;
        public UserEventConsumer(ILogger<UserEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<UserEvent> context)
        {
            _logger.LogInformation("Received UserId: {UserId}", context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
