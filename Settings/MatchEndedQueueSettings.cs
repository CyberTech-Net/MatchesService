namespace CyberTech.MatchesService.Settings
{
    public class MatchEndedQueueSettings
    {
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
    }
}
