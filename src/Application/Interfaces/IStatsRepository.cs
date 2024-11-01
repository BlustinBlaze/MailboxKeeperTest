using Application.DTOs.Stats;

namespace Application.Interfaces;

public interface IStatsRepository
{
    OrderStatsResponse GetStats();
}