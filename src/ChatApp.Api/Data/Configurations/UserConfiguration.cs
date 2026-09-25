using ChatApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(u => u.Id);

		builder.Property(u => u.Username).IsRequired().HasMaxLength(64);
		builder.HasIndex(u => u.Username).IsUnique();

		builder.Property(u => u.PasswordHash).IsRequired();
		builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(128);
		builder.Property(u => u.AvatarUrl).HasMaxLength(512);
		builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
	}
}
