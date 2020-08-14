using System.Collections.Generic;
using CluedIn.Crawling.Kafka.Core;

namespace CluedIn.Crawling.Kafka.Integration.Test
{
  public static class KafkaConfiguration
  {
    public static Dictionary<string, object> Create()
    {
      return new Dictionary<string, object>
            {
                { KafkaConstants.KeyName.ApiKey, "demo" }
            };
    }
  }
}
