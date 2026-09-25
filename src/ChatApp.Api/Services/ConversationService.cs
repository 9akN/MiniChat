using ChatApp.Api.DTOs.Conversations;
using ChatApp.Api.DTOs.Messages;
using ChatApp.Api.DTOs.Users;
using ChatApp.Api.Models;
using ChatApp.Api.Repositories.Interfaces;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Services;

public class ConversationService(IUnitOfWork unitOfWork) : IConversationService
{
	public async Task<IReadOnlyList<ConversationDto>> GetForUserAsync(int userId)
	{
		var conversationIds = await unitOfWork.ConversationUsers.Query()
			.Where(cu => cu.UserId == userId)
			.Select(cu => cu.ConversationId)
			.ToListAsync();

		var conversations = await unitOfWork.Conversations.Query()
			.Where(c => conversationIds.Contains(c.Id))
			.Include(c => c.ConversationUsers).ThenInclude(cu => cu.User)
			.Include(c => c.Messages)
			.OrderByDescending(c => c.Messages.OrderByDescending(m => m.SentAt).Select(m => m.SentAt).FirstOrDefault())
			.ToListAsync();

		return conversations.Select(c => MapToDto(c, userId)).ToList();
	}

	public async Task<ConversationDto> GetOrCreateAsync(int userId, int otherUserId)
	{
		if (userId == otherUserId)
			throw new InvalidOperationException("Cannot start a conversation with yourself.");

		var otherExists = await unitOfWork.Users.GetByIdAsync(otherUserId);
		if (otherExists is null)
			throw new InvalidOperationException("The other user does not exist.");

		var myConversationIds = unitOfWork.ConversationUsers.Query()
			.Where(cu => cu.UserId == userId)
			.Select(cu => cu.ConversationId);

		var existing = await unitOfWork.Conversations.Query()
			.Include(c => c.ConversationUsers).ThenInclude(cu => cu.User)
			.Include(c => c.Messages)
			.Where(c => myConversationIds.Contains(c.Id))
			.Where(c => c.ConversationUsers.Count == 2 && c.ConversationUsers.Any(cu => cu.UserId == otherUserId))
			.FirstOrDefaultAsync();

		if (existing is not null)
			return MapToDto(existing, userId);

		var conversation = new Conversation { CreatedAt = DateTime.UtcNow };
		conversation.ConversationUsers.Add(new ConversationUser { UserId = userId, JoinedAt = DateTime.UtcNow });
		conversation.ConversationUsers.Add(new ConversationUser { UserId = otherUserId, JoinedAt = DateTime.UtcNow });

		await unitOfWork.Conversations.AddAsync(conversation);
		await unitOfWork.SaveChangesAsync();

		var reloaded = await unitOfWork.Conversations.Query()
			.Include(c => c.ConversationUsers).ThenInclude(cu => cu.User)
			.Include(c => c.Messages)
			.FirstAsync(c => c.Id == conversation.Id);

		return MapToDto(reloaded, userId);
	}

	public async Task<bool> IsParticipantAsync(int conversationId, int userId) =>
		await unitOfWork.ConversationUsers.Query()
			.AnyAsync(cu => cu.ConversationId == conversationId && cu.UserId == userId);

	private static ConversationDto MapToDto(Conversation conversation, int currentUserId)
	{
		var lastMessage = conversation.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

		return new ConversationDto
		{
			Id = conversation.Id,
			CreatedAt = conversation.CreatedAt,
			Participants = conversation.ConversationUsers
				.Select(cu => new UserDto
				{
					Id = cu.User.Id,
					Username = cu.User.Username,
					DisplayName = cu.User.DisplayName,
					AvatarUrl = cu.User.AvatarUrl,
				})
				.ToList(),
			LastMessage = lastMessage is null
				? null
				: new MessageDto
				{
					Id = lastMessage.Id,
					ConversationId = lastMessage.ConversationId,
					SenderId = lastMessage.SenderId,
					SenderDisplayName = conversation.ConversationUsers.First(cu => cu.UserId == lastMessage.SenderId).User.DisplayName,
					Text = lastMessage.Text,
					SentAt = lastMessage.SentAt,
					IsRead = lastMessage.IsRead,
				},
			UnreadCount = conversation.Messages.Count(m => !m.IsRead && m.SenderId != currentUserId),
		};
	}
}
