using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using Amazon.SimpleNotificationService;
using AwsDomain;

namespace AwsWorker
{
    public class Program
    {
        public static readonly AmazonSQSConfig AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = "http://aws-localstack:4566" };
        public static AmazonSimpleNotificationServiceConfig AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://aws-localstack:4566" };


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    const string accessKey = "anaccesskey";
                    const string secretKey = "anaccesskey";

                    services.AddMassTransit(x =>
                    {
                        x.UsingAmazonSqs((context, cfg) =>
                        {
                            cfg.Host(new Uri("amazonsqs://aws-localstack:4566"), h =>
                            {
                                h.Config(AmazonSQSConfig);
                                h.Config(AmazonSnsConfig);
                                h.AccessKey(accessKey);
                                h.SecretKey(secretKey);

                                h.EnableScopedTopics();
                            });

                            cfg.Message<MessageTest>(x =>
                            {
                                x.SetEntityName("local-system-sns-topic");
                            });
                        });
                    });
                    services.AddHostedService<Worker>();
                });
    }
}
