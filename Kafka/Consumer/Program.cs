using Confluent.Kafka;
public class Program
{
    public static async Task Main(string[] args)
    {
        const string bootstrapServers = "localhost:9092"; // Your local Kafka broker address
        const string topicName = "my-topic";

        // Consumer example
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "my-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(topicName);
        var cts = new CancellationTokenSource();
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