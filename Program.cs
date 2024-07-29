using CyberTech.MatchesService.Consumers;
using CyberTech.MatchesService.Produsers;
using CyberTech.MatchesService.Settings;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace CyberTech.MatchesService
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connection = GetRabbitConnection(configuration);
            var connection2 = GetRabbitConnection(configuration);
            var applicationSettings = configuration.Get<ApplicationSettings>();
            var matchPlannedQueueSettings = applicationSettings.MatchPlannedQueueSettings;
            var matchEndedQueueSettings = applicationSettings.MatchEndedQueueSettings;
            MatchPlannedConsumer.Register(
                channel: connection.CreateModel(), 
                exchangeName: matchPlannedQueueSettings.ExchangeName,
                queueName: matchPlannedQueueSettings.QueueName,
                routingKey: matchPlannedQueueSettings.RoutingKey,
                matchEndedProduser: new MatchEndedProduser(
                    exchangeType: matchEndedQueueSettings.ExchangeType,
                    exchangeName: matchEndedQueueSettings.ExchangeName,
                    routingKey: matchEndedQueueSettings.RoutingKey,
                    durable: matchEndedQueueSettings.Durable,
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
