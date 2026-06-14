using BookGuess.Domain.Entities;

namespace BookGuess.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
}
