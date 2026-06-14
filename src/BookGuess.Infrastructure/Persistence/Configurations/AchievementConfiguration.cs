using BookGuess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookGuess.Infrastructure.Persistence.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(a => a.Code).IsUnique();
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Icon).HasMaxLength(100);
    }
}
