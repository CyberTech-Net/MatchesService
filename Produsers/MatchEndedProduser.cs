using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using CyberTech.MessagesContracts.TournamentMeets;
using CyberTech.MatchesService.Settings;

namespace CyberTech.MatchesService.Produsers
{
    internal class MatchEndedProduser
    {
        private readonly string _exchangeType;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly IModel _model;
        public MatchEndedProduser(MatchEndedQueueSettings queueSettings, IModel channel)
        {
            _exchangeName = queueSettings.ExchangeName;
            _exchangeType = queueSettings.ExchangeType;
            _routingKey = queueSettings.RoutingKey;
            _model = channel;
            _model.ExchangeDeclare(_exchangeName, _exchangeType, queueSettings.Durable);
        }

        public void Produce(MatchEnded message)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _model.BasicPublish(exchange: _exchangeName,
                routingKey: _routingKey,
                basicProperties: null,
                body: body);

            Console.WriteLine($"Match ended message is sent into exchange: {_exchangeName}");
        }
    }
}
