using AwsDomain;
using AwsDomain.Repository;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AwsReceiver
{
    public class MessageConsumer : IConsumer<MessageTest>
    {
        readonly ILogger<MessageConsumer> _logger;
        readonly IUserRepository _userRepository;
        public MessageConsumer( IUserRepository userRepository, ILogger<MessageConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MessageTest> context)
        {
            var userName = _userRepository.GetUserName();
            _logger.LogInformation("Received Text: {Text} from {userName}", context.Message.Text, userName);
            return Task.CompletedTask;
        }
    }
}
