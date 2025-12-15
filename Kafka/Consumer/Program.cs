using Confluent.Kafka;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapServers = "localhost:9092"; // Your local Kafka broker address
            var topicName = "my-topic";

            // Consumer example
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "my-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topicName);
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // Prevent the app from exiting immediately
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error consuming: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Consumer was cancelled
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }