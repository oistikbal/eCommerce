using System.Text;
using RabbitMQ.Client;

namespace eCommerce.Shared
{
    public class RabbitMQProducer
    {
        private IConnection _connection;
        private IChannel _channel;

        public async Task ConnectAsync(string hostName, string userName = "guest", string password = "guest")
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task SendMessageAsync(string exchange, string routingKey, string message)
        {
            await _channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Direct, durable: true);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }

        public void Disconnect()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
