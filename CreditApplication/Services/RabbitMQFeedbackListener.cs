using CreditApplication.Helpers;
using CreditApplication.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using IModel = RabbitMQ.Client.IModel;

namespace CoreApplication.BackgroundJobs
{
    public class RabbitMQFeedbackListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private IServiceProvider _provider;
        private readonly string _queueName;
        public RabbitMQFeedbackListener(IServiceProvider provider, IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMQ");
            _provider = provider;
            var connection = section["ReceiveConnection"];
            _queueName = section["ConfirmationQueue"];
            var factory = new ConnectionFactory { Uri = new Uri(connection) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<ConfirmationMessage>(content)
                    ?? throw new ArgumentNullException($"Failed to deserialize {nameof(ConfirmationMessage)} from Confirmation Queue");
                ConfirmationMessageFeedback.Instance.Receive(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
