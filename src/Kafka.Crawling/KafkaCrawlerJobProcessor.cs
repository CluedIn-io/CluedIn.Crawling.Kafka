using CluedIn.Crawling.Kafka.Core;

namespace CluedIn.Crawling.Kafka
{
    public class KafkaCrawlerJobProcessor : GenericCrawlerTemplateJobProcessor<KafkaCrawlJobData>
    {
        public KafkaCrawlerJobProcessor(KafkaCrawlerComponent component) : base(component)
        {
        }
    }
}
