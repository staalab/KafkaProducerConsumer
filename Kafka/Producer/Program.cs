using Confluent.Kafka;
public class Program
{
    public static async Task Main(string[] args)
    {
        const string bootstrapServers = "localhost:9092"; // Your local Kafka broker address
        const string topicName = "my-topic";

        // Producer example
        var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers, ClientId = topicName };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        var message = new Message<Null, string> { Value = "Hello Kafka from .NET!" };
        var n = 0;
        while (n<10)
        {
            n += 1;
            var deliveryReport = await producer.ProduceAsync(topicName, message);
            Console.WriteLine($"Delivered '{deliveryReport.Value}' to '{deliveryReport.TopicPartitionOffset}'");
        }
    }
}