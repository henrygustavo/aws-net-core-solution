using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AwsWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    //Reading configuration
                    var aswSection = _configuration.GetSection("Aws");
                    var accessKey = aswSection.GetSection("AccessKey").Value;
                    var secretKey = aswSection.GetSection("SecretKey").Value;
                    var sqsUrl = aswSection.GetSection("SQSUrl").Value;
                    var serviceUrl = aswSection.GetSection("ServiceUrl").Value;


                    var credentials = new AnonymousAWSCredentials();

                    //var credentials = new SessionAWSCredentials(accessKey, accessKey, accessKey);
                    //var credentials = new BasicAWSCredentials(accessKey, accessKey);
                    //var chain = new CredentialProfileStoreChain();
                    //var result = chain.TryGetAWSCredentials("default", out var credentials);

                    //Creating sqs client
                    var config = new AmazonSQSConfig
                    {
                        ServiceURL = serviceUrl,
                        // RegionEndpoint = RegionEndpoint.USWest1,
                    };

                    var ss = config.ServiceURL;

                    Console.WriteLine(ss);
                    AmazonSQSClient amazonSQSClient = new AmazonSQSClient(credentials, config);

                    //Receive request
                    ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest
                    {
                        QueueUrl = sqsUrl
                    };
                    var response = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                    if (response.Messages.Any())
                    {
                        foreach (Message message in response.Messages)
                        {
                            Console.WriteLine($"Message received");
                            Console.WriteLine($"Message: {message.Body}");

                            //Deleting message
                            var deleteMessageRequest = new DeleteMessageRequest(sqsUrl, message.ReceiptHandle);
                            await amazonSQSClient.DeleteMessageAsync(deleteMessageRequest, stoppingToken);

                            Console.WriteLine($"Message deleted");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
