namespace CyberTech.MatchesService.Settings
{
    public class ApplicationSettings
    {
        public RmqSettings RmqSettings { get; set; }
        public MatchPlannedQueueSettings MatchPlannedQueueSettings { get; set; }
        public MatchEndedQueueSettings MatchEndedQueueSettings { get; set; }
    }
}