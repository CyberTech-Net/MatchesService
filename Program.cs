using CyberTech.MatchesService.Consumers;
using CyberTech.MatchesService.Produsers;
using CyberTech.MatchesService.Settings;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;

namespace CyberTech.MatchesService
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connection = GetRabbitConnection(configuration);
            var connection2 = GetRabbitConnection(configuration);
            var applicationSettings = configuration.Get<ApplicationSettings>();
            var matchPlannedQueueSettings = applicationSettings.MatchPlannedQueueSettings;
            var matchEndedQueueSettings = applicationSettings.MatchEndedQueueSettings;
            MatchPlannedConsumer.Register(
                channel: connection.CreateModel(),
                queueSettings: matchPlannedQueueSettings,
                matchEndedProduser: new MatchEndedProduser(
                    queueSettings: matchEndedQueueSettings,
                    channel: connection2.CreateModel()));

        }
        
        private static IConnection GetRabbitConnection(IConfiguration configuration)
        {
            var rmqSettings = configuration.Get<ApplicationSettings>().RmqSettings;
            var factory = new ConnectionFactory
            {
                HostName = rmqSettings.Host,
                VirtualHost = rmqSettings.VHost,
                UserName = rmqSettings.Login,
                Password = rmqSettings.Password,
            };
            return factory.CreateConnection();
        }
    }
}
