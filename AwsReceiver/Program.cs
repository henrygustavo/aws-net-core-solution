using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using AwsDomain.Repository;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleNotificationService;
using AwsReceiver.Config;
using Consumers.AwsReceiver;
using AwsReceiver.Consumers;

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
                    var AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = _awsConfing.ServiceUrl };
                    var AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL = _awsConfing.ServiceUrl };

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageEventConsumer>();
                        x.AddConsumer<UserEventConsumer>();

                        x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.Host(new Uri(_awsConfing.HostUrl), h =>
                        {
                            h.Config(AmazonSQSConfig);
                            h.Config(AmazonSnsConfig);
                            h.AccessKey(_awsConfing.AccessKey);
                            h.SecretKey(_awsConfing.SecretKey);
                        });

                        cfg.ReceiveEndpoint(queueName: _awsConfing.Queue, e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.Subscribe(_awsConfing.MessageEventTopic, s => { });
                            e.Subscribe(_awsConfing.UserEventTopic, s => { });
                            e.ConfigureConsumer<MessageEventConsumer>(context);
                            e.ConfigureConsumer<UserEventConsumer>(context);
                        });
                    });
                    });
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddHostedService<Worker>();
                });
    }
}
