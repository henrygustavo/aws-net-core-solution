using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using AwsDomain.Repository;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleNotificationService;
using AwsReceiver.Config;

namespace AwsReceiver
{
    public class Program
    {
        public static AwsConfig _awsConfing;


        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json", true, true).Build();

            _awsConfing = configuration.GetSection("Aws").Get<AwsConfig>();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = _awsConfing.ServiceUrl};
                    var AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL =_awsConfing.ServiceUrl};

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();

                        x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.Host(new Uri(_awsConfing.HostUrl), h =>
                        {
                            h.Config(AmazonSQSConfig);
                            h.Config(AmazonSnsConfig);
                            h.AccessKey(_awsConfing.AccessKey);
                            h.SecretKey(_awsConfing.SecretKey);

                            h.EnableScopedTopics();
                        });

                        cfg.ReceiveEndpoint(queueName: _awsConfing.Queue, e =>
                        {
                            e.Subscribe(_awsConfing.Topic, s => { });
                            e.ConfigureConsumer<MessageConsumer>(context);
                        });
                    });
                    });
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddMassTransitHostedService(waitUntilStarted: true);
                    services.AddHostedService<Worker>();
                });
    }
}
