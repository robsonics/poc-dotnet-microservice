using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace WebApplication1.Services
{

    public class MessageClient: IMessageClient
    {
        private ConfigData _configData;

        public MessageClient(IOptions<ConfigData> configData)
        {
            _configData = configData.Value;
        }

        public void ReciveMsg(Action<string> msgHandler)
        {
            var factory = new ConnectionFactory() { HostName = _configData.RabbitMqIp };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configData.InputQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    msgHandler(message);
                };
                channel.BasicConsume(queue: _configData.InputQueue,
                                     consumer: consumer);
             
            }
        }
    }

    public interface IMessageClient
    {
        void ReciveMsg(Action<string> msgHandler);
    }
}
