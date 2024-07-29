using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using CyberTech.MessagesContracts.TournamentMeets;

namespace CyberTech.MatchesService.Produsers
{
    internal class MatchEndedProduser
    {
        private string _exchangeType;
        private string _exchangeName;
        private string _routingKey;
        private IModel _model;
        public MatchEndedProduser(string exchangeType, string exchangeName, string routingKey, bool durable,IModel channel)
        {
            _exchangeName = exchangeName;
            _exchangeType = exchangeType;
            _routingKey = routingKey;
            _model = channel;
            _model.ExchangeDeclare(_exchangeName, _exchangeType, durable);
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
