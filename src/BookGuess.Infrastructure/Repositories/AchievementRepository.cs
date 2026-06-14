using BookGuess.Domain.Entities;
using BookGuess.Domain.Interfaces;
using BookGuess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookGuess.Infrastructure.Repositories;

public class AchievementRepository(BookGuessDbContext context) : IAchievementRepository
{
    public async Task<Achievement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await context.Achievements.FirstOrDefaultAsync(a => a.Code == code, cancellationToken);

    public async Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Achievements.ToListAsync(cancellationToken);

    public async Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task AddUserAchievementAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default) =>
        await context.UserAchievements.AddAsync(userAchievement, cancellationToken);

    public async Task<bool> UserHasAchievementAsync(Guid userId, string code, CancellationToken cancellationToken = default) =>
        await context.UserAchievements
            .Include(ua => ua.Achievement)
            .AnyAsync(ua => ua.UserId == userId && ua.Achievement.Code == code, cancellationToken);
}
