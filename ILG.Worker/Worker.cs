using Confluent.Kafka;
using ILG.Domain.Models;
using System.Text.Json;
using ILG.Worker.Data;

namespace ILG.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ConsumerConfig _config;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        // Configuração do Consumidor
        _config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "logistics-group", // Identifica este grupo de trabalhadores
            AutoOffsetReset = AutoOffsetReset.Earliest // Se for novo, lê desde o início
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
    consumer.Subscribe("order-received");

    _logger.LogInformation("Worker aguardando cargas do Kafka...");

    while (!stoppingToken.IsCancellationRequested)
    {
        try 
        {
            var consumeResult = consumer.Consume(stoppingToken);

            if (consumeResult != null)
            {
                // 1. EXTRAIR O JSON E TRANSFORMAR EM OBJETO C#
                // consumeResult.Message.Value contém o texto JSON enviado pela API
                var order = JsonSerializer.Deserialize<CargoOrder>(consumeResult.Message.Value);

                if (order != null) 
                {
                    using var dbContext = new AppDbContext();
                    await dbContext.Database.EnsureCreatedAsync(stoppingToken);

                    _logger.LogInformation($"[KAFKA -> DB]: Persistindo carga {order.Id}...");
                    
                    // 2. SALVAR O OBJETO 'order', NÃO O 'consumeResult'
                    dbContext.Orders.Add(order);
                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation($"[SUCESSO]: Carga {order.Id} salva no banco de dados.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao processar mensagem: {ex.Message}");
        }
    }
}
}