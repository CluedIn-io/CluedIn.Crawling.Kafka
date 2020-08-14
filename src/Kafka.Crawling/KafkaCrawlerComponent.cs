using CluedIn.Core;
using CluedIn.Crawling.Kafka.Core;

using ComponentHost;

namespace CluedIn.Crawling.Kafka
{
    [Component(KafkaConstants.CrawlerComponentName, "Crawlers", ComponentType.Service, Components.Server, Components.ContentExtractors, Isolation = ComponentIsolation.NotIsolated)]
    public class KafkaCrawlerComponent : CrawlerComponentBase
    {
        public KafkaCrawlerComponent([NotNull] ComponentInfo componentInfo)
            : base(componentInfo)
        {
        }
    }
}

