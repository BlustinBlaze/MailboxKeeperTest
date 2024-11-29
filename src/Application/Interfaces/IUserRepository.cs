using API;
using Application.DTOs.Notifications;

namespace Application.Interfaces;

public interface IUserRepository
{
    User CreateUser(UserDto userDto);
    User? GetUserByEmail(string email);
    User UpdateMailboxUser(int idMailbox, int id);
    User GetMailboxIdByUser(int id);
    void UpdateUser(string fcmtoken, int id);
    void LogoutUser(int userId);
    User GetUserByMailboxId(int id);
    bool VerifyPassword(string email, string password);
    bool UpdateNotification(bool notification, int id);
}