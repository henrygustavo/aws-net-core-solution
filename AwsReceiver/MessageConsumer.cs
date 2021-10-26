using AwsEntity;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AwsReceiver
{
    public class MessageConsumer : IConsumer<MessageTest>
    {
        readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer() { }

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MessageTest> context)
        {
           // _logger.LogInformation("Received Text: {Text}", context.Message.Text);
            Console.WriteLine(context.Message.Text);
            return Task.CompletedTask;
        }
    }
}
