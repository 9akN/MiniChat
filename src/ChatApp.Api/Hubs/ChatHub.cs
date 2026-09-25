using System.Security.Claims;
using ChatApp.Api.DTOs.Messages;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs;

[Authorize]
public class ChatHub(IConversationService conversationService, IMessageService messageService) : Hub
{
	private int CurrentUserId => int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

	public override async Task OnConnectedAsync()
	{
		var conversations = await conversationService.GetForUserAsync(CurrentUserId);
		foreach (var conversation in conversations)
			await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversation.Id));

		await base.OnConnectedAsync();
	}

	public async Task JoinConversation(int conversationId)
	{
		if (!await conversationService.IsParticipantAsync(conversationId, CurrentUserId))
			throw new HubException("You are not a participant of this conversation.");

		await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
	}

	public async Task SendMessage(int conversationId, string text)
	{
		if (!await conversationService.IsParticipantAsync(conversationId, CurrentUserId))
			throw new HubException("You are not a participant of this conversation.");

		MessageDto message = await messageService.SendAsync(conversationId, CurrentUserId, text);

		await Clients.Group(GroupName(conversationId)).SendAsync("ReceiveMessage", message);
	}

	public async Task MarkAsRead(int conversationId)
	{
		await messageService.MarkAsReadAsync(conversationId, CurrentUserId);
		await Clients.Group(GroupName(conversationId)).SendAsync("MessagesRead", conversationId, CurrentUserId);
	}

	private static string GroupName(int conversationId) => $"conversation-{conversationId}";
}
