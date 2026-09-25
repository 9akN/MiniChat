using ChatApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Api.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
	public void Configure(EntityTypeBuilder<Conversation> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
	}
}
