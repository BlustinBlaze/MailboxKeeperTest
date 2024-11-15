using Application.DTOs.Test;

namespace Application.Interfaces;

public interface ITestRepository
{
    TestResponse GetTest();
}