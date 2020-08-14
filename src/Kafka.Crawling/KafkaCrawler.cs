using System.Collections.Generic;

using CluedIn.Core.Crawling;
using CluedIn.Crawling.Kafka.Core;
using CluedIn.Crawling.Kafka.Infrastructure.Factories;

namespace CluedIn.Crawling.Kafka
{
    public class KafkaCrawler : ICrawlerDataGenerator
    {
        private readonly IKafkaClientFactory clientFactory;
        public KafkaCrawler(IKafkaClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public IEnumerable<object> GetData(CrawlJobData jobData)
        {
            if (!(jobData is KafkaCrawlJobData kafkacrawlJobData))
            {
                yield break;
            }

            var client = clientFactory.CreateNew(kafkacrawlJobData);

            //retrieve data from provider and yield objects
            
        }       
    }
}
