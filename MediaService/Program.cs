using MediaService;
using MediaService.Data;
using MediaService.Repositories;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

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
    return connection;
});

builder.Services.AddSingleton<RabbitMQListener>(); // Add RabbitMQListener as a singleton

var app = builder.Build();

// Initialize RabbitMQListener to start listening for UID messages
var rabbitMQListener = app.Services.GetRequiredService<RabbitMQListener>();
rabbitMQListener.StartListeningForUid("uid_queue");

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
