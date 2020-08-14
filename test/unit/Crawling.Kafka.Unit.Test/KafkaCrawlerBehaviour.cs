using CluedIn.Core.Crawling;
using CluedIn.Crawling;
using CluedIn.Crawling.Kafka;
using CluedIn.Crawling.Kafka.Infrastructure.Factories;
using Moq;
using Should;
using Xunit;

namespace Crawling.Kafka.Unit.Test
{
    public class KafkaCrawlerBehaviour
    {
        private readonly ICrawlerDataGenerator _sut;

        public KafkaCrawlerBehaviour()
        {
            var nameClientFactory = new Mock<IKafkaClientFactory>();

            _sut = new KafkaCrawler(nameClientFactory.Object);
        }

        [Fact]
        public void GetDataReturnsData()
        {
            var jobData = new CrawlJobData();

            _sut.GetData(jobData)
                .ShouldNotBeNull();
        }
    }
}
