using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Notifications;
using Application.Exceptions;
using Application.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class MailboxController(IConfiguration configuration, IMailboxRepository mailboxRepository, IUserRepository userRepository, ILogger<MailboxController> logger, IHubContext<NotificationHub, INotificationClient> hubContext) : ControllerBase
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public ActionResult GetAllMailboxes()
    {
        logger.LogInformation("Getting mailboxes");
        try
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            var mailboxId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(mailboxId))
            {
                logger.LogWarning("User ID or Mailbox ID is not present in the token");
                return Unauthorized("User ID or Mailbox ID is missing");
            }
            var mailboxes = mailboxRepository.GetMailboxes(userId);
            hubContext.Clients.Group(mailboxId).ReceiveHistory(mailboxes);
            return StatusCode(200, mailboxes);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Mailboxes not found");
            return NotFound();
        }
    }
    
    [HttpGet("{id:int}")]
    public ActionResult<Mailbox> GetMailboxById(int id)
    {
        logger.LogInformation("Getting mailbox by id");
        try
        {
            var mailbox = mailboxRepository.GetMailboxById(id);
            hubContext.Clients.Group(mailbox.Id.ToString()).ReceiveMailbox(mailbox);
            return StatusCode(200, mailbox);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Mailbox not found");
            return NotFound();
        }
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public ActionResult CreateMailbox(CreateMailboxRequest createMailbox)
    {
        logger.LogInformation("Creating mailbox");
        try
        {
            mailboxRepository.CreateMailbox(createMailbox);
            return StatusCode(201);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the mailbox.");
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<ActionResult> UpdateMailbox(UpdateMailboxRequest updateMailbox)
    {
        logger.LogInformation("Updating mailbox");
        try
        {
            var mailboxId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(mailboxId))
            {
                logger.LogWarning("User ID is not present in the token");
                return Unauthorized("User ID is missing");
            }
            var newMailbox = mailboxRepository.UpdateMailbox(updateMailbox);
            await hubContext.Clients.Group(mailboxId).ReceiveMailbox(newMailbox);

            var user = userRepository.GetUserByMailboxId(newMailbox.Id);
            logger.LogInformation("user notification is {notification}", user.Notification);
            if (user.Fcmtoken == null || user.Fcmtoken == "" || user.Notification != true)
            {
                logger.LogInformation("User has no FCM token or notification disabled.");
                return StatusCode(200);
            }
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Mailbox Updated",
                    Body = "Your mailbox has been updated."
                },
                Token = user.Fcmtoken
            };
            
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            if (!string.IsNullOrEmpty(result))
            {
                // Message was sent successfully
                Console.WriteLine("Message sent successfully!");
            }
            else
            {
                // There was an error sending the message
                Console.WriteLine("Error sending the message.");
            }
            
            return StatusCode(200);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Mailbox not found");
            return NotFound();
        }
    }
    
    [HttpPost]
    [Route("Login")]
    public ActionResult LoginMailbox(MailboxResponse model)
    {
        logger.LogInformation("Logging in mailbox with id {id}", model.Id);
        try
        {
            var mailbox = mailboxRepository.GetMailboxById(model.Id);
            if (mailbox == null)
            {
                return BadRequest("Mailbox not found");
            }
            if (mailbox.Password != model.Password)
            {
                return BadRequest("Invalid password");
            }
            logger.LogInformation("Mailbox logged in successfully.");
            return Ok(new { Token = GenerateJwtToken(mailbox) });
        } catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while logging in the mailbox.");
            return StatusCode(500, ex.Message);
        }
    }
    
    private string GenerateJwtToken(MailboxResponse mailbox)
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
                new(ClaimTypes.NameIdentifier, mailbox.Id.ToString()),
                new(ClaimTypes.SerialNumber, mailbox.Password)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}