using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MediaService
{
    public class RabbitMQService
    {
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMQService(RabbitMQ.Client.IModel channel)
        {
            _channel = channel;
        }

        public void SendMessage(string message)
        {
            // Send a message
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "tweets_queue", basicProperties: null, body: body);

            Console.WriteLine($" [x] Sent '{message}'");
        }
    }
}
