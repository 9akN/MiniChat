using ChatApp.Api.Models;

namespace ChatApp.Api.Repositories.Interfaces;

public interface IUnitOfWork
{
	IRepository<User> Users { get; }
	IRepository<Conversation> Conversations { get; }
	IRepository<ConversationUser> ConversationUsers { get; }
	IRepository<Message> Messages { get; }

	Task<int> SaveChangesAsync();
}
