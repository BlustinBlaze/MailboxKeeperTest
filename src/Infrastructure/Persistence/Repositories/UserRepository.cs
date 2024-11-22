using API;
using Application.DTOs.Notifications;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApiContext db, IMapper mapper, ILogger<UserRepository> logger): IUserRepository
{
    public User CreateUser(UserDto userDto)
    {
        var user = mapper.Map<User>(userDto);
        var existingUser = GetUserByEmail(user.Email);
        if (existingUser != null)
        {
            logger.LogError("User with email {email} already exists.", user.Email);
            return null;
        }
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        db.Users.Add(user);
        db.SaveChanges();
        logger.LogInformation("User added to database successfully.");
        return user;
    }
    
    public User? GetUserByEmail(string email)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            logger.LogError("User with email {email} not found.", email);
            return null;
        }
        return user;
    }
    
    public User? GetUserById(int id)
    {
        var user = db.Users.Find(id);
        if (user == null)
        {
            logger.LogError("User with id {id} not found.", id);
            return null;
        }
        return user;
    }

    public User UpdateMailboxUser(int idMailbox, int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            logger.LogError("User with id {id} not found.", id);
            return null;
        }
        var mailbox = db.Mailboxes.Find(idMailbox);
        if (mailbox == null)
        {
            logger.LogError("Mailbox with id {id} not found.", idMailbox);
            return null;
        }

        user.IdMailbox = idMailbox;
        db.SaveChanges();
        logger.LogInformation("Mailbox updated for user with email {email}.", user.Email);
        return user;
    }
    
    public User GetMailboxIdByUser(int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            logger.LogError("User with id {id} not found.", id);
            return null;
        }
        return user;
    }
    
    public void UpdateUser(string fcmtoken, int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            logger.LogError("User with id {id} not found.", id);
            return;
        }
        user.Fcmtoken = fcmtoken;
        db.SaveChanges();
        logger.LogInformation("User updated successfully.");
    }
    
    public void LogoutUser(int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            logger.LogError("User with id {id} not found.", id);
            return;
        }
        user.Fcmtoken = null;
        db.SaveChanges();
        logger.LogInformation("User logged out successfully.");
    }
    
    public User GetUserByMailboxId(int id)
    {
        var user = db.Users.FirstOrDefault(u => u.IdMailbox == id);
        if (user == null)
        {
            logger.LogError("User with mailbox id {id} not found.", id);
            return null;
        }
        return user;
    }

    public bool VerifyPassword(string email, string password)
    {
        var user = GetUserByEmail(email);
        if (user == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}