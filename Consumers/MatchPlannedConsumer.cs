using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CyberTech.MatchesService.Produsers;
using CyberTech.MessagesContracts.TournamentMeets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CyberTech.MatchesService.Consumers
{
    internal static class MatchPlannedConsumer
    {
        private static Random rand = new();
        public static void Register(IModel channel, string exchangeName, string queueName, string routingKey, MatchEndedProduser matchEndedProduser)
        {
                channel.BasicQos(0, 10, false);
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.QueueBind(queueName, exchangeName, routingKey, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, e) =>
                {
                    var body = Encoding.UTF8.GetString(e.Body.ToArray());
                    var bodyFixed = body.Substring(1, body.Length - 2).Replace("\\u0022", "\"");
                    var message = JsonSerializer.Deserialize<MatchPlanned>(bodyFixed);
                    Console.WriteLine($"{DateTime.Now} Received message: {message.Id}");
                    channel.BasicAck(e.DeliveryTag, false);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    var (firstTeam, secondTeam) = GenerateResult(message);

                    matchEndedProduser.Produce(firstTeam);
                    matchEndedProduser.Produce(secondTeam);
                };

                channel.BasicConsume(queueName, false, consumer);
                Console.WriteLine($"Subscribed to the queue with key {routingKey} (exchange name: {exchangeName})");
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