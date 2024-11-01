using Application.DTOs.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace API;

public class ServerNotifier : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ILogger<ServerNotifier> _logger;

    public ServerNotifier(IHubContext<NotificationHub, INotificationClient> hubContext, ILogger<ServerNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Sending notification to clients");
            await _hubContext.Clients.All.ReceiveHistory(new List<HistoryResponse>
            {
                new()
                {
                    Id = 1,
                    Status = "Open",
                    //Time = DateTime.Now,
                    MailWeight = 0.5
                }
            });
        }
    }
}