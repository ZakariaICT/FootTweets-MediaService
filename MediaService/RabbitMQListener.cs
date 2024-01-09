using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MediaService.Model;

namespace MediaService
{
    public class RabbitMQListener
    {
        private readonly IConnection _connection;

        public RabbitMQListener(IConnection connection)
        {
            _connection = connection;
        }

        public void StartListeningForUid(string queueName)
        {
            using (var channel = _connection.CreateModel())
            {
                // Similar to the RabbitMQConsumer, set up a listener for the UID queue
                // Use a method like StartListening in RabbitMQConsumer to consume UID and create tweets
                // Ensure you call this method during the service startup

                // Example:
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var uid = Encoding.UTF8.GetString(body);

                    // Create a new tweet using the UID
                    CreateTweetWithUid(uid);
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine($"Listening to RabbitMQ UID queue: {queueName}");
            }
        }

        private void CreateTweetWithUid(string uid)
        {
            // Assume you have a method to create a new tweet in your application
            // Pass the UID to this method when creating a new tweet
            // Adjust this according to your application's logic

            var tweetDetails = new Pictures
            {
                Text = "Your tweet text here",
                PictureURL = "Your picture URL here",
                Uid = uid,  // Use the UID obtained from RabbitMQ
            };

            // Call your method to create a new tweet
            CreateNewTweet(tweetDetails);
        }

        private void CreateNewTweet(Pictures tweetDetails)
        {
            // Implement the logic to create a new tweet in your application
            // Use tweetDetails.Id as the user ID when creating the tweet

            // Example:
            Console.WriteLine($"Creating a new tweet for user ID: {tweetDetails.Uid}");
            Console.WriteLine($"Text: {tweetDetails.Text}, PictureURL: {tweetDetails.PictureURL}");
        }
    }
}
