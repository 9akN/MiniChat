using ChatApp.Api.DTOs.Messages;
using ChatApp.Api.Models;
using ChatApp.Api.Repositories.Interfaces;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Services;

public class MessageService(IUnitOfWork unitOfWork) : IMessageService
{
	public async Task<IReadOnlyList<MessageDto>> GetForConversationAsync(int conversationId) =>
		await unitOfWork.Messages.Query()
			.Where(m => m.ConversationId == conversationId)
			.Include(m => m.Sender)
			.OrderBy(m => m.SentAt)
			.Select(m => new MessageDto
			{
				Id = m.Id,
				ConversationId = m.ConversationId,
				SenderId = m.SenderId,
				SenderDisplayName = m.Sender.DisplayName,
				Text = m.Text,
				SentAt = m.SentAt,
				IsRead = m.IsRead,
			})
			.ToListAsync();

	public async Task<MessageDto> SendAsync(int conversationId, int senderId, string text)
	{
		var sender = await unitOfWork.Users.GetByIdAsync(senderId)
			?? throw new InvalidOperationException("Sender does not exist.");

		var message = new Message
		{
			ConversationId = conversationId,
			SenderId = senderId,
			Text = text,
			SentAt = DateTime.UtcNow,
			IsRead = false,
		};

		await unitOfWork.Messages.AddAsync(message);
		await unitOfWork.SaveChangesAsync();

		return new MessageDto
		{
			Id = message.Id,
			ConversationId = message.ConversationId,
			SenderId = message.SenderId,
			SenderDisplayName = sender.DisplayName,
			Text = message.Text,
			SentAt = message.SentAt,
			IsRead = message.IsRead,
		};
	}

	public async Task MarkAsReadAsync(int conversationId, int readerId)
	{
		var unread = await unitOfWork.Messages.FindAsync(
			m => m.ConversationId == conversationId && m.SenderId != readerId && !m.IsRead);

		foreach (var message in unread)
			message.IsRead = true;

		if (unread.Count > 0)
			await unitOfWork.SaveChangesAsync();
	}
}
