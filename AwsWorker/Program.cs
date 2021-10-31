using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using Amazon.SimpleNotificationService;
using AwsDomain;
using Microsoft.Extensions.Configuration;
using AwsWorker.Config;

namespace AwsWorker
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
                                    x.UsingAmazonSqs((context, cfg) =>
                                    {
                                        cfg.Host(new Uri(_awsConfing.HostUrl), h =>
                                        {
                                            h.Config(AmazonSQSConfig);
                                            h.Config(AmazonSnsConfig);
                                            h.AccessKey(_awsConfing.AccessKey);
                                            h.SecretKey(_awsConfing.SecretKey);
                                        });

                                        cfg.Message<MessageEvent>(x =>
                                        {
                                            x.SetEntityName(_awsConfing.MessageEventTopic);
                                        });
                                        cfg.Message<UserEvent>(x =>
                                        {
                                            x.SetEntityName(_awsConfing.UserEventTopic);
                                        });
                                    });
                                });
                    services.AddHostedService<Worker>();
                });
    }
}
