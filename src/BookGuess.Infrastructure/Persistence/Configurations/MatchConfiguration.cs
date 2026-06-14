using BookGuess.Domain.Entities;
using BookGuess.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookGuess.Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Status).HasConversion<string>();
        builder.Property(m => m.Score).HasDefaultValue(Match.BaseScore);

        builder.HasMany(m => m.Guesses)
            .WithOne(g => g.Match)
            .HasForeignKey(g => g.MatchId);

        builder.HasOne(m => m.BookQuote)
            .WithMany()
            .HasForeignKey(m => m.BookQuoteId);
    }
}
