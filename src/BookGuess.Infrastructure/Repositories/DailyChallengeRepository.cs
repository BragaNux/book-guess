using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class DailyChallengeRepository(BookGuessDbContext context) : IDailyChallengeRepository
{
    public async Task<DailyChallenge?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default) =>
        await context.DailyChallenges
            .Include(dc => dc.BookQuote)
                .ThenInclude(q => q.Book)
            .FirstOrDefaultAsync(dc => dc.ChallengeDate == date, cancellationToken);

    public async Task AddAsync(DailyChallenge challenge, CancellationToken cancellationToken = default) =>
        await context.DailyChallenges.AddAsync(challenge, cancellationToken);
}
