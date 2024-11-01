using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Notifications;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class MailboxController(IConfiguration configuration ,IMailboxRepository mailboxRepository, ILogger<MailboxController> logger, IHubContext<NotificationHub, INotificationClient> hubContext) : ControllerBase
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public ActionResult GetAllMailboxes()
    {
        logger.LogInformation("Getting mailboxes");
        try
        {
            var mailboxes = mailboxRepository.GetMailboxes();
            hubContext.Clients.All.ReceiveHistory(mailboxes);
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
            hubContext.Clients.All.ReceiveMailbox(mailbox);
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
    public ActionResult UpdateMailbox(UpdateMailboxRequest updateMailbox)
    {
        logger.LogInformation("Updating mailbox");
        try
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID is not present in the token");
                return Unauthorized("User ID is missing");
            }
            var newMailbox = mailboxRepository.UpdateMailbox(updateMailbox);
            hubContext.Clients.Group(userId).ReceiveMailbox(newMailbox);
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