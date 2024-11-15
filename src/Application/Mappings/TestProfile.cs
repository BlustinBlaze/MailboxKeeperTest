using Application.DTOs.Test;
using Application.Entities.Test;
using AutoMapper;

namespace Application.Mappings;

public class TestProfile: Profile
{
    public TestProfile()
    {
        CreateMap<Test, TestResponse>();
    }
}