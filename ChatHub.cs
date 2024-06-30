using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessageToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("SendMessageNotification", message);
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }
	public async Task JoinGroup(string groupName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
	}

	public async Task LeaveGroup(string groupName)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
	}

	public async Task SendMessageToGroup(string groupName, string message)
	{
		await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", Context.ConnectionId, message);
	}

	public override async Task OnConnectedAsync()
	{
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		await base.OnDisconnectedAsync(exception);
	}
}
