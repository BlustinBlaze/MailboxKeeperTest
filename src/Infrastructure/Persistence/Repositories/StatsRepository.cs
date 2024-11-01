using System.Text.Json;
using Application.DTOs.Stats;
using Application.Entities.Stats;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class StatsRepository(IMapper mapper, ILogger<StatsRepository> logger) : IStatsRepository
{
    public OrderStatsResponse GetStats()
    {
        logger.LogInformation("Getting order stats...");
        const string fileName = "order_stats.json";
        if (!File.Exists(fileName))
        {
            logger.LogError("Order stats file not found");
            throw new NotFoundException("Order stats not found");
        }
        var orderStatsJson = File.ReadAllText(fileName);
        var orderStats = JsonSerializer.Deserialize<OrderStats>(orderStatsJson);
        logger.LogInformation("Order stats retrieved successfully");
        return mapper.Map<OrderStatsResponse>(orderStats);
    }
}