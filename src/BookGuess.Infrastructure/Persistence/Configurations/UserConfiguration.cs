using BookGuess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookGuess.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.Level).HasDefaultValue(1);
        builder.Property(u => u.XP).HasDefaultValue(0);
        builder.Property(u => u.CurrentStreak).HasDefaultValue(0);
        builder.Property(u => u.BestStreak).HasDefaultValue(0);
        builder.Property(u => u.IsActive).HasDefaultValue(true);

        builder.HasMany(u => u.Matches)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        builder.HasMany(u => u.UserAchievements)
            .WithOne(ua => ua.User)
            .HasForeignKey(ua => ua.UserId);
    }
}
