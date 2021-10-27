using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Amazon.SQS;
using Amazon.SimpleNotificationService;
using AwsDomain;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace AwsWorker
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

                    var AmazonSQSConfig = new AmazonSQSConfig { ServiceURL = serviceUrl };
                    var AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl };

                    services.AddMassTransit(x =>
                                {
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

                                        cfg.Message<MessageTest>(x =>
                                        {
                                            x.SetEntityName(topic);
                                        });
                                    });
                                });
                    services.AddHostedService<Worker>();
                });
    }
}
