using ChatApp.Api.Data.Configurations;
using ChatApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<User> Users => Set<User>();
	public DbSet<Conversation> Conversations => Set<Conversation>();
	public DbSet<ConversationUser> ConversationUsers => Set<ConversationUser>();
	public DbSet<Message> Messages => Set<Message>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new ConversationConfiguration());
		modelBuilder.ApplyConfiguration(new ConversationUserConfiguration());
		modelBuilder.ApplyConfiguration(new MessageConfiguration());
	}
}
