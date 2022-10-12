using Microsoft.AspNetCore.SignalR;

namespace issuetracker.Hubs;

public class ChatHub : Hub
{
	public async Task JoinGroup(string groupName)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
	}
	public async Task SendMessage(string user, string message, string groupName)
	{
		await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
	}
}