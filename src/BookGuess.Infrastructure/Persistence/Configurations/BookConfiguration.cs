using BookGuess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookGuess.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Genre).HasMaxLength(100);
        builder.Property(b => b.Description).HasMaxLength(2000);
        builder.Property(b => b.Difficulty).IsRequired();

        builder.HasMany(b => b.Quotes)
            .WithOne(q => q.Book)
            .HasForeignKey(q => q.BookId);

        builder.HasMany(b => b.Trivias)
            .WithOne(t => t.Book)
            .HasForeignKey(t => t.BookId);
    }
}
