using ChatApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Api.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
	public void Configure(EntityTypeBuilder<Message> builder)
	{
		builder.HasKey(m => m.Id);

		builder.Property(m => m.Text).IsRequired().HasMaxLength(4000);
		builder.Property(m => m.SentAt).HasDefaultValueSql("GETUTCDATE()");
		builder.Property(m => m.IsRead).HasDefaultValue(false);

		builder.HasOne(m => m.Conversation)
			.WithMany(c => c.Messages)
			.HasForeignKey(m => m.ConversationId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(m => m.Sender)
			.WithMany(u => u.Messages)
			.HasForeignKey(m => m.SenderId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
