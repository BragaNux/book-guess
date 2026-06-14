using BookGuess.Domain.Entities;

namespace BookGuess.Domain.Interfaces;

public interface IAchievementRepository
{
    Task<Achievement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddUserAchievementAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default);
    Task<bool> UserHasAchievementAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}
