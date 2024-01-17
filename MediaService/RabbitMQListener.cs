using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using MediaService.Model;
using Microsoft.Extensions.DependencyInjection;
using MediaService.DTO;
using Newtonsoft.Json;

namespace MediaService
{
    public class RabbitMQListener
    {
        public string GetUidFromQueue(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQConnection"]),
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "uid_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                string uid = null;

                consumer.Received += (model, ea) =>
                {
                    var bodyBytes = ea.Body.ToArray();
                    uid = Encoding.UTF8.GetString(bodyBytes);

                    // Note: No manual acknowledgment, let RabbitMQ handle it automatically
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Set autoAck to true
                channel.BasicConsume(queue: "uid_queue", autoAck: false, consumer: consumer);

                return uid;
            }
        }




        public void ProcessRabbitMQMessages(IConfiguration configuration, Action<PictureReadDTO> processMessageLocally)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQConnection"]),
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "tweets_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var bodyBytes = ea.Body.ToArray();
                    var pictureJson = Encoding.UTF8.GetString(bodyBytes);
                    var pictureDTO = JsonConvert.DeserializeObject<PictureReadDTO>(pictureJson);

                    processMessageLocally(pictureDTO);
                };

                channel.BasicConsume(queue: "tweets_queue", autoAck: true, consumer: consumer);
            }
        }



    }

}
