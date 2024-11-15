using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationHub() : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Client connected");
        var userId = Context.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
        Console.WriteLine("User: " + userId);
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("Client disconnected");
        var userId = Context.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnDisconnectedAsync(exception);
    }
    
    
}