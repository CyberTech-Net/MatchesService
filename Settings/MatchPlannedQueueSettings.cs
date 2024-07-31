namespace CyberTech.MatchesService.Settings
{
    public class MatchPlannedQueueSettings
    {
        public string ExchangeName { get; set; }
        public string Type { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
