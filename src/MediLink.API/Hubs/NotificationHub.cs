namespace MediLink.API.Hubs;

using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task Subscribe() => await Clients.Caller.SendAsync("Subscribed");
}
