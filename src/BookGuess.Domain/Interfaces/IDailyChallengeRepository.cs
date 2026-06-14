using BookGuess.Domain.Entities;

namespace BookGuess.Domain.Interfaces;

public interface IDailyChallengeRepository
{
    Task<DailyChallenge?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task AddAsync(DailyChallenge challenge, CancellationToken cancellationToken = default);
}
