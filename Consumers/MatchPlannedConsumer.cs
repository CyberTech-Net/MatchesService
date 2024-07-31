using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CyberTech.MatchesService.Produsers;
using CyberTech.MatchesService.Settings;
using CyberTech.MessagesContracts.TournamentMeets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CyberTech.MatchesService.Consumers
{
    internal static class MatchPlannedConsumer
    {
        private static Random rand = new();
        public static void Register(IModel channel, MatchPlannedQueueSettings queueSettings, MatchEndedProduser matchEndedProduser)
        {
            channel.ExchangeDeclare(queueSettings.ExchangeName, queueSettings.Type, queueSettings.Durable, queueSettings.AutoDelete, null);
            channel.BasicQos(0, 10, false);
            channel.QueueDeclare(queueSettings.QueueName, queueSettings.Durable, false, queueSettings.AutoDelete, null);
            channel.QueueBind(queueSettings.QueueName, queueSettings.ExchangeName, queueSettings.RoutingKey, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                var message = JsonSerializer.Deserialize<MatchPlanned>(body);
                Console.WriteLine($"{DateTime.Now} Received message: {message.Id}");
                channel.BasicAck(e.DeliveryTag, false);
                await Task.Delay(TimeSpan.FromSeconds(2));
                var (firstTeam, secondTeam) = GenerateResult(message);

                matchEndedProduser.Produce(firstTeam);
                matchEndedProduser.Produce(secondTeam);
            };

            channel.BasicConsume(queueSettings.QueueName, false, consumer);
            Console.WriteLine($"Subscribed to the queue {queueSettings.QueueName} with key {queueSettings.RoutingKey} (exchange name: {queueSettings.ExchangeName})");
            Console.ReadLine();
        }

        public static (MatchEnded firstTeam, MatchEnded secondTeam) GenerateResult(MatchPlanned message)
        {
            var firstTeam = new MatchEnded(
                        matchId: message.Id,
                        teamId: message.FirtstTeamId,
                        score: rand.Next(0,10),
                        isWin: false);

            var secondTeam = new MatchEnded(
                        matchId: message.Id,
                        teamId: message.FirtstTeamId,
                        score: rand.Next(0, 10),
                        isWin: false);

            firstTeam.IsWin = firstTeam.Score > secondTeam.Score;
            secondTeam.IsWin = !firstTeam.IsWin;

            return (firstTeam, secondTeam);
        }
    }
}