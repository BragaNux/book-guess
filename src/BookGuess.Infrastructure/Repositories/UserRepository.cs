using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class UserRepository(BookGuessDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Users.FindAsync([id], cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await context.Users.AddAsync(user, cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<User>> GetTopRankingAsync(int limit, CancellationToken cancellationToken = default) =>
        await context.Users
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.XP)
            .Take(limit)
            .ToListAsync(cancellationToken);
}
