using System.Text.Json;
using Application.DTOs.Test;
using Application.Entities.Test;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class TestRepository(IMapper mapper, ILogger<TestRepository> logger): ITestRepository
{
    public TestResponse GetTest()
    {
        logger.LogInformation("Getting test info...");
        const string fileName = "test.json";
        if (!File.Exists(fileName))
        {
            logger.LogError("Test file not found");
            throw new NotFoundException("Test not found");
        }
        var testJson = File.ReadAllText(fileName);
        var test = JsonSerializer.Deserialize<Test>(testJson);
        logger.LogInformation("Test retrieved successfully");
        return mapper.Map<TestResponse>(test);
    }
}