using Confluent.Kafka;
using ILG.Domain.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint para receber a carga
app.MapPost("/orders", async (CargoOrder order) => {
    // No nível Pleno, usamos o 'using' para garantir que o recurso seja liberado
    using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    
    // Transformamos nosso objeto C# em um texto JSON para o Kafka entender
    var messageValue = JsonSerializer.Serialize(order);
    
    // Enviamos para o tópico 'order-received'
    var result = await producer.ProduceAsync("order-received", 
        new Message<Null, string> { Value = messageValue });

    return Results.Accepted($"/orders/{order.Id}", new { 
        Status = "Enviado para o Kafka", 
        Offset = result.Offset.Value 
    });
});

app.Run();