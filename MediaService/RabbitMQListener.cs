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

        private readonly IServiceProvider _serviceProvider;

        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMQListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RabbitMQListener(IServiceProvider serviceProvider, IConnection connection, IModel channel)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _channel = channel;
        }

        public RabbitMQListener(IConnection connection, IModel channel, IMediaRepo mediaRepo)
        {
            _connection = connection;
            _channel = channel;
            _mediaRepo = mediaRepo;
        }

        public RabbitMQListener()
        { 
            
        }

        private IMediaRepo GetMediaRepo()
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IMediaRepo>();
        }

        public void StartListening(IConfiguration configuration)
        {
            Console.WriteLine("RabbitMQListener is now listening for user deletion messages...");
            if (_channel == null)
            {
                Console.WriteLine("_channel is null. Ensure it is properly initialized.");
                return;
            }

            _channel.QueueDeclare(queue: "user.deleted",
                      durable: true,  // Change this to match the existing queue
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);


            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                HandleUserDeletion(ea, configuration);
            };

            _channel.BasicConsume(queue: "user.deleted",
                                   autoAck: true,
                                   consumer: consumer);
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


        public void deleteUsersTweets(IConnection _connection, IModel _channel, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQConnection"]),
            };

            // Initialize RabbitMQ connection and channel here (similar to previous code)
            // ...
            _channel.QueueDeclare(queue: "user.deleted",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                // Obtain the JSON message from the user.deletion queue
                byte[] messageBodyBytes = ea.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);

                // Log that a user deletion request has been received
                Console.WriteLine("Received user deletion request:");

                // Deserialize the JSON message to extract the UserId
                var messageObject = JsonConvert.DeserializeObject<Pictures>(messageBody);
                string uidAuth = messageObject.UidAuth;

                // Log the UID obtained from the message
                Console.WriteLine($"UID: {uidAuth}");

                // Delete media associated with the user ID from the database
                _mediaRepo.DeletePicturesByUserId(uidAuth);

                // Log that media deletion has occurred
                Console.WriteLine($"Media related to User with ID {uidAuth} deleted.");
            };

            _channel.BasicConsume(queue: "user.deleted",
                                   autoAck: true,
                                   consumer: consumer);
        }

        public void HandleUserDeletion(BasicDeliverEventArgs e, IConfiguration configuration)
        {
            try
            {
                byte[] messageBodyBytes = e.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);
                Console.WriteLine($"Received message: {messageBody}");

                // Assuming the message format is just a simple JSON with UserId
                var messageObject = JsonConvert.DeserializeObject<UserDeletionMessage>(messageBody); // Replace with the correct class
                if (messageObject == null)
                {
                    Console.WriteLine("Deserialization of message failed.");
                    return;
                }

                string userId = messageObject.UserId; // Replace with the correct property
                Console.WriteLine($"Processing deletion for User ID: {userId}");

                // Check if _mediaRepo is null
                //if (_mediaRepo == null)
                //{
                //    Console.WriteLine("_mediaRepo is null. Repository is not initialized.");
                //    return;
                //}
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediaRepo = scope.ServiceProvider.GetRequiredService<IMediaRepo>();
                    mediaRepo.DeletePicturesByUserId(userId); // This now uses a fresh context
                }
                Console.WriteLine($"Media related to User with ID {userId} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling user deletion message: {ex.Message}");
            }
        }




    }

}
