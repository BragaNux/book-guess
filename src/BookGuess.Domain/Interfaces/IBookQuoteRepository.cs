using BookGuess.Domain.Entities;

namespace BookGuess.Domain.Interfaces;

public interface IBookQuoteRepository
{
    Task<BookQuote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookQuote?> GetRandomAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BookQuote quote, CancellationToken cancellationToken = default);
}
