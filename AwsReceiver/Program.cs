using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using AwsDomain.Repository;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleNotificationService;

namespace AwsReceiver
{
    public class Program
    {
        public static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json", true, true).Build();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    string accessKey = _configuration["Aws:AccessKey"];
                    string secretKey = _configuration["Aws:SecretKey"];
                    string serviceUrl = _configuration["Aws:ServiceUrl"];
                    string hostUrl = _configuration["Aws:HostUrl"];
                    string topic = _configuration["Aws:Topic"];
                    string queue = _configuration["Aws:Queue"];

                    var AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = serviceUrl };
                    var AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl };

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();

                        x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.Host(new Uri(hostUrl), h =>
                        {
                            h.Config(AmazonSQSConfig);
                            h.Config(AmazonSnsConfig);
                            h.AccessKey(accessKey);
                            h.SecretKey(secretKey);

                            h.EnableScopedTopics();
                        });

                        cfg.ReceiveEndpoint(queueName: queue, e =>
                        {
                            e.Subscribe(topic, s => { });
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
