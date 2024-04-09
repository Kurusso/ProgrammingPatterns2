using CoreApplication.Configurations;
using CoreApplication.Models.DTO;
using CoreApplication.Models.Enumeration;
using CoreApplication.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using IModel = RabbitMQ.Client.IModel;

namespace CoreApplication.BackgroundJobs
{
    public class OperationsListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private IModel _deliveryConfirmationChannel;
        private IServiceProvider _provider;
        private Uri _rabbitMqConnection;
        private readonly RabbitMqConfigurations _rabbitMqConfigurations;
        public OperationsListener(IServiceProvider provider, IOptions<RabbitMqConfigurations> rabbitMqConfigurations)
        {

            _rabbitMqConfigurations = rabbitMqConfigurations.Value;
            _rabbitMqConnection = rabbitMqConfigurations.Value.Connection;
            _provider = provider;
            var factory = new ConnectionFactory { Uri = _rabbitMqConnection };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _deliveryConfirmationChannel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _rabbitMqConfigurations.QueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _deliveryConfirmationChannel.ExchangeDeclare(exchange: _rabbitMqConfigurations.SecondQueName, type: "direct", durable: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<OperationPostDTO>(content);
                using (var scope = _provider.CreateScope())
                {
                    var scopedService = scope.ServiceProvider.GetRequiredService<IMoneyOperationsService>();
                    var confirmationMessage = new ConfirmationDTO();
                    confirmationMessage.MessageTrackNumber = message?.Id.ToString() ?? ea.DeliveryTag.ToString();
                    try
                    {
                        if (message != null)
                        {

                            switch (message.OperationType)
                            {
                                case OperationType.Deposit:
                                    await scopedService.Deposit(message.MoneyAmmount, message.Currency, message.AccountId, message.UserId);
                                    break;
                                case OperationType.Withdraw:
                                    await scopedService.Withdraw(message.MoneyAmmount, message.Currency, message.AccountId, message.UserId);
                                    break;
                                case OperationType.TransferSend:
                                    await scopedService.TransferMoney(message.MoneyAmmount, message.Currency, message.AccountId, message.UserId, (Guid)message.RecieverAccount);
                                    break;

                            }

                        }

                        confirmationMessage.Message = "";
                        confirmationMessage.Status = 200;
                        
                    }
                    catch (InvalidOperationException ex)
                    {
                        confirmationMessage.Message = ex.Message;
                        confirmationMessage.Status = 400;
                    }
                    catch (ArgumentException ex)
                    {
                        confirmationMessage.Message = ex.Message;
                        confirmationMessage.Status = 400;
                    }
                    catch(KeyNotFoundException ex)
                    {
                        confirmationMessage.Message = ex.Message;
                        confirmationMessage.Status = 404;
                    }
                    catch (Exception ex)
                    {
                        confirmationMessage.Message = ex.Message;
                        confirmationMessage.Status = 500;
                    }
                    var deliveryConfirmationMessage = JsonConvert.SerializeObject(confirmationMessage);
                    _deliveryConfirmationChannel.BasicPublish(exchange: "",
                                                                  routingKey: _rabbitMqConfigurations.SecondQueName,
                                                                  basicProperties: null,
                                                                  body: Encoding.UTF8.GetBytes(deliveryConfirmationMessage));
                }
                _channel.BasicAck(ea.DeliveryTag, false);

            };

            _channel.BasicConsume(_rabbitMqConfigurations.QueName, false, consumer);

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
