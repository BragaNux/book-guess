using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class BookRepository(BookGuessDbContext context) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Books
            .Include(b => b.Quotes)
            .Include(b => b.Trivias)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Books.ToListAsync(cancellationToken);

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default) =>
        await context.Books.AddAsync(book, cancellationToken);
}
