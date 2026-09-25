using ChatApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Api.Data.Configurations;

public class ConversationUserConfiguration : IEntityTypeConfiguration<ConversationUser>
{
	public void Configure(EntityTypeBuilder<ConversationUser> builder)
	{
		builder.HasKey(cu => new { cu.ConversationId, cu.UserId });

		builder.HasOne(cu => cu.Conversation)
			.WithMany(c => c.ConversationUsers)
			.HasForeignKey(cu => cu.ConversationId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(cu => cu.User)
			.WithMany(u => u.ConversationUsers)
			.HasForeignKey(cu => cu.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(cu => cu.JoinedAt).HasDefaultValueSql("GETUTCDATE()");
	}
}
