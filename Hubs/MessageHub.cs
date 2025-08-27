using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Api.Models;

namespace Api.Hubs;

public interface IMessagesClient
{
    Task MessageCreated(Message message);
    Task MessageUpdated(Message message);
    Task MessageDeleted(int id);
    Task MessageRead(int id);
    Task UnreadCountChanged(int count);
}

// Quitando autorización para permitir SignalR sin autenticación
public class MessageHub : Hub<IMessagesClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, "broadcast");

        await base.OnConnectedAsync();
    }
}
