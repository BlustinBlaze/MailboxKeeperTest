using API;
using Application.DTOs.Notifications;
using AutoMapper;

namespace Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserRequest, User>();
        CreateMap<UserDto, User>();
    }
}