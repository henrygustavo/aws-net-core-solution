using AwsDomain;
using AwsDomain.Repository;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Consumers.AwsReceiver
{
    public class MessageEventConsumer : IConsumer<MessageEvent>
    {
        readonly ILogger<MessageEventConsumer> _logger;
        readonly IUserRepository _userRepository;
        public MessageEventConsumer( IUserRepository userRepository, ILogger<MessageEventConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MessageEvent> context)
        {
            var userName = _userRepository.GetUserName();
            _logger.LogInformation("Received Text: {Text} from {userName}", context.Message.Text, userName);
            return Task.CompletedTask;
        }
    }
}
