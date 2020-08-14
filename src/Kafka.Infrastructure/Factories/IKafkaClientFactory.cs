using CluedIn.Crawling.Kafka.Core;

namespace CluedIn.Crawling.Kafka.Infrastructure.Factories
{
    public interface IKafkaClientFactory
    {
        KafkaClient CreateNew(KafkaCrawlJobData kafkaCrawlJobData);
    }
}
