using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class BookQuoteRepository(BookGuessDbContext context) : IBookQuoteRepository
{
    public async Task<BookQuote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.BookQuotes
            .Include(q => q.Book)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

    public async Task<BookQuote?> GetRandomAsync(CancellationToken cancellationToken = default)
    {
        var count = await context.BookQuotes.CountAsync(cancellationToken);
        if (count == 0) return null;

        var skip = Random.Shared.Next(0, count);
        return await context.BookQuotes
            .Include(q => q.Book)
            .Skip(skip)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(BookQuote quote, CancellationToken cancellationToken = default) =>
        await context.BookQuotes.AddAsync(quote, cancellationToken);
}
