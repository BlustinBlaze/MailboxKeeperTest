using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class TestController(ILogger<TestController> logger, ITestRepository testRepository): ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        logger.LogInformation("Getting json test info");
        try
        {
            var testMsg = testRepository.GetTest();
            logger.LogInformation("JSON test retrieved successfully");
            return StatusCode(200, testMsg);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "JSON not found");
            return NotFound();
        }
    }
}