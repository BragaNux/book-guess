using BookGuess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookGuess.Infrastructure.Persistence.Configurations;

public class GuessConfiguration : IEntityTypeConfiguration<Guess>
{
    public void Configure(EntityTypeBuilder<Guess> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.GuessText).IsRequired().HasMaxLength(300);
        builder.Property(g => g.GuessType).HasConversion<string>();
    }
}
