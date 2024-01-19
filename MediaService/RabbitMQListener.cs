using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using MediaService.Model;
using Microsoft.Extensions.DependencyInjection;
using MediaService.DTO;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Threading.Channels;
using MediaService.Repositories;

namespace MediaService
{
    public class RabbitMQListener
    {
        private readonly IMediaRepo _mediaRepo;

        public RabbitMQListener(IMediaRepo mediaRepo)
        {
            _mediaRepo = mediaRepo;
        }

        public RabbitMQListener()
        { 
            
        }
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


        public void deleteUsersTweets(IConnection _connection, IModel _channel)
        {
            // Initialize RabbitMQ connection and channel here (similar to previous code)
            // ...
            _channel.QueueDeclare(queue: "user.deletion",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                HandleUserDeletion(ea);
            };

            _channel.BasicConsume(queue: "user.deletion",
                                   autoAck: true,
                                   consumer: consumer);
        }



        public void HandleUserDeletion(BasicDeliverEventArgs e)
        {
            try
            {
                // Convert the message body (ReadOnlyMemory<byte>) to a byte[]
                byte[] messageBodyBytes = e.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);
                string userId = messageBody;

                // Delete media associated with the user ID from the database
                _mediaRepo.DeletePicturesByUserId(userId);

                Console.WriteLine($"Media related to User with ID {userId} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling user deletion message: {ex.Message}");
            }
        }



    }

}
