using BookGuess.Domain.Entities;

namespace BookGuess.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Match?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Match match, CancellationToken cancellationToken = default);
    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);
    Task<int> CountWonByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
