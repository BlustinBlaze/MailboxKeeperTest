using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class StatsController(IStatsRepository statsRepository, ILogger<StatsController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult GetStats()
    {
        logger.LogInformation("Getting stats");
        try
        {
            var stats = statsRepository.GetStats();
            logger.LogInformation("Stats retrieved successfully");
            return StatusCode(200, stats);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Stats not found");
            return NotFound();
        }
    }
}