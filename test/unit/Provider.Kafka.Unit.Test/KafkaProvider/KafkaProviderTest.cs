using Castle.Windsor;
using CluedIn.Core;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Kafka.Infrastructure.Factories;
using Moq;

namespace CluedIn.Provider.Kafka.Unit.Test.KafkaProvider
{
    public abstract class KafkaProviderTest
    {
        protected readonly ProviderBase Sut;

        protected Mock<IKafkaClientFactory> NameClientFactory;
        protected Mock<IWindsorContainer> Container;

        protected KafkaProviderTest()
        {
            Container = new Mock<IWindsorContainer>();
            NameClientFactory = new Mock<IKafkaClientFactory>();
            var applicationContext = new ApplicationContext(Container.Object);
            Sut = new Kafka.KafkaProvider(applicationContext, NameClientFactory.Object);
        }
    }
}
