using ChatApp.Api.Data;
using ChatApp.Api.Models;
using ChatApp.Api.Repositories.Interfaces;

namespace ChatApp.Api.Repositories;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;

	public UnitOfWork(AppDbContext context)
	{
		_context = context;
		Users = new Repository<User>(context);
		Conversations = new Repository<Conversation>(context);
		ConversationUsers = new Repository<ConversationUser>(context);
		Messages = new Repository<Message>(context);
	}

	public IRepository<User> Users { get; }
	public IRepository<Conversation> Conversations { get; }
	public IRepository<ConversationUser> ConversationUsers { get; }
	public IRepository<Message> Messages { get; }

	public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
