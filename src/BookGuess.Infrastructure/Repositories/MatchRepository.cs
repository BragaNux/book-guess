using BookGuess.Domain.Entities;
using BookGuess.Domain.Enums;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class MatchRepository(BookGuessDbContext context) : IMatchRepository
{
    public async Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.BookQuote)
                .ThenInclude(q => q.Book)
            .Include(m => m.Guesses)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Match?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.Matches
            .Include(m => m.BookQuote)
                .ThenInclude(q => q.Book)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == MatchStatus.Active, cancellationToken);

    public async Task AddAsync(Match match, CancellationToken cancellationToken = default) =>
        await context.Matches.AddAsync(match, cancellationToken);

    public Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        context.Matches.Update(match);
        return Task.CompletedTask;
    }

    public async Task<int> CountWonByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.Matches.CountAsync(m => m.UserId == userId && m.Status == MatchStatus.Won, cancellationToken);
}
