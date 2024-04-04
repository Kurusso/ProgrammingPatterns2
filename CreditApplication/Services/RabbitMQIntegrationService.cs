using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace CreditApplication.Services
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj);
        void SendMessage(string message);
    }
    public class RabbitMQIntegrationService : IRabbitMqService
    {
        private readonly string _rabbitMQConnection;
        private readonly string _queueTransactions;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RabbitMQIntegrationService(IConfiguration configuration, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            var section = configuration.GetSection("RabbitMQ");
            _rabbitMQConnection = section["HostName"];
            _queueTransactions = section["TransactionQueue"];
            _jsonSerializerOptions = jsonSerializerOptions.Value;
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = _rabbitMQConnection };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueTransactions,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                           routingKey: _queueTransactions,
                           basicProperties: null,
                           body: body);
            }
        }
    }
}
