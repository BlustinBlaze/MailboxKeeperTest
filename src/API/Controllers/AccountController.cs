using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Notifications;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class AccountController(IConfiguration configuration, ILogger<AccountController> logger, IUserRepository userRepository) : ControllerBase
{
    [Route("Create")]
    [HttpPost]
    public ActionResult CreateUser(UserDto model)
    {
        logger.LogInformation("Creating a new user");
        try
        {
            var user = userRepository.CreateUser(model);
            logger.LogInformation("User created with email {email}", user.Email);
            return Ok(new { Token = GenerateJwtToken(user) });
        } catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the user.");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("Login")]
    public ActionResult Login(UserDto userInfo)
    {
        logger.LogInformation("Logging in user with email {email}", userInfo.Email);
        try
        {
            var user = userRepository.GetUserByEmail(userInfo.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            if (user.Password != userInfo.Password)
            {
                return BadRequest("Invalid password");
            }
            if (userInfo.Fcmtoken != null)
            {
                userRepository.UpdateUser(userInfo.Fcmtoken, user.Id);
            }
            logger.LogInformation("User logged in successfully.");
            return Ok(new { Token = GenerateJwtToken(user) });
        } catch (Exception ex) {
            logger.LogError(ex, "An error occurred while logging in the user.");
            return StatusCode(500, ex.Message);
        }
    }
    
    [Route("UpdateMailbox")]
    [HttpPost]
    public ActionResult UpdateMailboxUser(int idMailbox, int id)
    {
        logger.LogInformation("Updating mailbox for user with id {id}", id);
        try
        {
            var user = userRepository.UpdateMailboxUser(idMailbox, id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            logger.LogInformation("Mailbox updated successfully.");
            return Ok();
        } catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the mailbox.");
            return StatusCode(500, ex.Message);
        }
    }
    
    [Route("Mailbox")]
    [HttpGet]
    public ActionResult GetMailboxUser(int id)
    {
        logger.LogInformation("Getting mailbox for user with id {id}", id);
        try
        {
            var user = userRepository.GetMailboxIdByUser(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            logger.LogInformation("Mailbox retrieved successfully.");
            return Ok(user.IdMailbox);
        } catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving the mailbox.");
            return StatusCode(500, ex.Message);
        }
    }
    
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = configuration["Jwt:Key"];

        if (key == null)
            throw new Exception("Jwt key is not set");
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.SerialNumber, user.IdMailbox.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}