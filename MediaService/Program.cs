using MediaService;
using MediaService.Data;
using MediaService.Repositories;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using MediaService.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Media API", Version = "v1" });
});
builder.Services.AddScoped<IMediaRepo, MediaRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add RabbitMQ services
builder.Services.AddSingleton(serviceProvider =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(configuration["RabbitMQConnection"]),
    };

    var connection = factory.CreateConnection();

    // Provide the necessary variables and methods to handle the UID
    var pictureRepository = serviceProvider.GetRequiredService<IMediaRepo>();

    // Provide the callback function for handling the UID
    Action<string> onUidReceived = uid =>
    {
        // Use the received UID to handle the creation of a new picture
        var pictureModel = new Pictures(); // Replace with your actual logic
        pictureModel.Uid = uid;

        // Save the updated picture model to the database
        pictureRepository.CreatePicture(pictureModel);
        pictureRepository.saveChanges();
    };

    // Provide the callback function for handling the tweet request
    Action<string> onTweetReceived = tweetRequest =>
    {
        // Process the tweet request
        Console.WriteLine($"Received tweet request: {tweetRequest}");
        // Implement your logic to handle the tweet request
    };

    // Create and return RabbitMQListener with both callback functions
    return new RabbitMQListener();
});

var app = builder.Build();

// Initialize RabbitMQListener to start listening for UID messages
var rabbitMQListener = app.Services.GetRequiredService<RabbitMQListener>();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
