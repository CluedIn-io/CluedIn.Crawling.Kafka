using CluedIn.Core.Crawling;

namespace CluedIn.Crawling.Kafka.Core
{
    public class KafkaCrawlJobData : CrawlJobData
    {
        public string ApiKey { get; set; }
        public string KafkaServer { get; set; }
        public string KafkaConnectionString { get; set; }
        public string KafkaTopic { get; set; }
        public int KafkaDummyClueGenerationInterval { get; set; }
    }
}
