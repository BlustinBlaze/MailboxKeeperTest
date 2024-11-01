using API;
using Application.DTOs.Notifications;

namespace Application.Interfaces;

public interface IUserRepository
{
    User CreateUser(UserDto userDto);
    User? GetUserByEmail(string email);
    User UpdateMailboxUser(int idMailbox, int id);
    User GetMailboxIdByUser(int id);
}