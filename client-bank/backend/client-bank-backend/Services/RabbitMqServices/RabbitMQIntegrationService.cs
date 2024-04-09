using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace client_bank_backend.Services.RabbitMqServices
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj);
        void SendMessage(string message);
    }

    public class ConfirmationMessage
    {
        public int Status { get; set; }
        public string MessageTrackNumber { get; set; }
        public string Message { get; set; }
    }

    public class RabbitMQIntegrationService : IRabbitMqService, IDisposable
    {
        private readonly string _rabbitMQConnection;
        private readonly string _queueTransactions;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQIntegrationService(IConfiguration configuration, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            var section = configuration.GetSection("RabbitMQ");
            _rabbitMQConnection = section["Connection"];
            _queueTransactions = section["TransactionQueue"];
            _jsonSerializerOptions = jsonSerializerOptions.Value;
            var factory = new ConnectionFactory() { Uri = new Uri(_rabbitMQConnection) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueTransactions,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "ClientApplication",//ClientApplication
                       routingKey: _queueTransactions,
                       basicProperties: null,
                       body: body);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
