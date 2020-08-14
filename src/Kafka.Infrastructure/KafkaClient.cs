using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CluedIn.Core.Logging;
using CluedIn.Core.Providers;
using CluedIn.Crawling.Kafka.Core;
using Confluent.Kafka;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using RestSharp;

namespace CluedIn.Crawling.Kafka.Infrastructure
{
    // TODO - This class should act as a client to retrieve the data to be crawled.
    // It should provide the appropriate methods to get the data
    // according to the type of data source (e.g. for AD, GetUsers, GetRoles, etc.)
    // It can receive a IRestClient as a dependency to talk to a RestAPI endpoint.
    // This class should not contain crawling logic (i.e. in which order things are retrieved)
    public class KafkaClient
    {
        private const string BaseUri = "http://sample.com";

        private readonly ILogger log;

        private readonly IRestClient client;

        private readonly KafkaCrawlJobData _kafkaCrawlJobData;

        private static ConsumerConfig _consumerConfig = null;
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        public KafkaClient(ILogger log, KafkaCrawlJobData kafkaCrawlJobData, IRestClient client) // TODO: pass on any extra dependencies
        {
            if (kafkaCrawlJobData == null)
            {
                throw new ArgumentNullException(nameof(kafkaCrawlJobData));
            }

            _kafkaCrawlJobData = kafkaCrawlJobData;

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            // TODO use info from kafkaCrawlJobData to instantiate the connection
            client.BaseUrl = new Uri(BaseUri);
            client.AddDefaultParameter("api_key", kafkaCrawlJobData.ApiKey, ParameterType.QueryString);

            if (!string.IsNullOrEmpty(_kafkaCrawlJobData.KafkaServer)) //this means that Kafka stream is not configured, so we should use the API. If it's not empty/null, then it's configured so we should use it.
            {
                //Configure the consumer
                _consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = _kafkaCrawlJobData.KafkaServer,

                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SslCaLocation = @".\cacert.pem",
                    SocketTimeoutMs = 60000,
                    SessionTimeoutMs = 30000,
                    SaslUsername = @"$ConnectionString",

                    SaslPassword = _kafkaCrawlJobData.KafkaConnectionString,

                    GroupId = @"$Default",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
            }
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var request = new RestRequest(url, Method.GET);

            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var diagnosticMessage = $"Request to {client.BaseUrl}{url} failed, response {response.ErrorMessage} ({response.StatusCode})";
                log.Error(() => diagnosticMessage);
                throw new InvalidOperationException($"Communication to jsonplaceholder unavailable. {diagnosticMessage}");
            }

            var data = JsonConvert.DeserializeObject<T>(response.Content);

            return data;
        }

        public AccountInformation GetAccountInformation()
        {
            //TODO - return some unique information about the remote data source
            // that uniquely identifies the account
            return new AccountInformation("", ""); 
        }

        public IEnumerable<object> GetKafkaObjets()
        {
            IConsumer<byte[], string> consumer = null;

            try
            {
                try
                {
                    //creating the consumer
                    consumer = new ConsumerBuilder<byte[], string>(_consumerConfig).SetKeyDeserializer(Deserializers.ByteArray).Build();
                    //subscribing to the topic from the jobData
                    consumer.Subscribe(this._kafkaCrawlJobData.KafkaTopic);
                }
                catch (Exception ex)
                {
                    this.log.Error(() => $"Kafka Crawler - Could not create Consumer. Exception: {ex.Message}");
                    if (consumer != null)
                        consumer.Dispose();
                    yield break;
                }

                //track the time spent in the loop
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var attempt = 1;
                while (!_cts.IsCancellationRequested)
                {
                    ConsumeResult<byte[], string> cr = null;
                    try
                    {
                        cr = consumer.Consume(new TimeSpan(0));
                        attempt = 1;
                    }
                    catch (Exception ex)
                    {
                        this.log.Error(() => $"Kafka Crawler - Could not consume. Exception: {ex.Message}. Attempt {attempt}.");
                        attempt++;
                        if (attempt > 3)
                        {
                            this.log.Error(() => $"Kafka Crawler - Could not consume. Exception: {ex.Message}. Ending loop.");
                            _cts.Cancel();
                        }
                        else
                            Thread.Sleep(1000);

                        continue;
                    }

                    if (cr?.Message != null)
                    {
                        stopWatch.Restart(); //if we found a message, we restart the timer to 0
                        var resource = this.GetTypedObject(cr);

                        if (resource == null)
                            continue;
                        yield return resource;
                    }
                    else if (stopWatch.Elapsed.TotalMinutes >= this._kafkaCrawlJobData.KafkaDummyClueGenerationInterval) //in order to prevent the job from being shut down, we create a dummy clue and then we restart the timer
                    {
                        stopWatch.Restart();
                        this.log.Info(() => $"Kafka Crawler - Creating dummy object after receiving 0 messages for 5 minutes.");

                        yield return new IssContact
                        {
                            AccountId = $"Dummy AccountId",
                            AccountIdName = $"Dummy AccountIdName",
                            AccountIdYomiName = $"Dummy AccountIdName",
                            AccountRoleCode = $"Dummy AccountIdName",
                            AccountRoleCodeName = $"Dummy AccountIdName",
                            ContactId = $"Dummy ContactId",
                            Description = $"Dummy Description",
                            FullName = $"Dummy FullName",
                            NickName = $"Dummy NickName",
                        };
                    }
                }
                consumer.Dispose();
            }
            finally
            {
                if (consumer != null)
                {
                    consumer.Dispose();
                }
            }
        }
        private object GetTypedObject(ConsumeResult<byte[], string> cr)
        {
            try
            {
                switch (_kafkaCrawlJobData.KafkaTopic.ToLower())
                {
                    case "crm.contact":
                        return ExtractObject<IssContact>(cr.Message.Value); //to confirm with ISS
                    case "crm.customer":
                        return ExtractObject<IssAccount>(cr.Message.Value); //to confirm with ISS
                    default:
                        {
                            this._log.Error(() => $"Kafka Crawler - Could not get object form message. Topic {cr.Topic}. Offset: {cr.TopicPartitionOffset}");
                            return null;
                        }
                }
            }
            catch (Exception ex)
            {
                this.log.Error(() => $"Kafka Crawler - Could not get object form message. Topic {cr.Topic}. Offset: {cr.TopicPartitionOffset}. Exception: {ex.Message}");
                return null;
            }
        }
        public static T ExtractObject<T>(string jsonString)
        {
            var remoteExecutionContext = DeserializeJsonString<RemoteExecutionContext>(jsonString);

            var attributes = remoteExecutionContext.PostEntityImages.Values.First().Attributes;
            var jsonStringContent = string.Join(",", attributes.Select(a => $"\"{a.Key}\": \"{a.Value}\"").ToArray());

            var newObjectifiedJsonString = "{" + jsonStringContent + "}";

            var obj = JsonConvert.DeserializeObject<T>(newObjectifiedJsonString);

            return obj;
        }
        public static RemoteContextType DeserializeJsonString<RemoteContextType>(string jsonString)
        {
            var obj = Activator.CreateInstance<RemoteContextType>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (RemoteContextType)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
