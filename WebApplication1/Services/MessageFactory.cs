using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace WebApplication1.Services
{
    public class MessageFactory: IMessageFactory
    {
        private IOptions<ConfigData> _configData;

        public MessageFactory(IOptions<ConfigData> configData)
        {
            _configData = configData;
        }

        public void StartGeneratingMsg(string prefix)
        {
            var factory = new ConnectionFactory() { HostName = _configData.Value.RabbitMqIp };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configData.Value.OutputQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

               
                for (int i = 0; i < 3; i++)
                {
                    string message = i+".Hello World! ver" + _configData.Value.Version+" " + prefix;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                   routingKey: _configData.Value.OutputQueue,
                                   basicProperties: null,
                                   body: body);
                }
              
            }
        }
    }

    public interface IMessageFactory
    {
        void StartGeneratingMsg(string prefix);
    }
}
