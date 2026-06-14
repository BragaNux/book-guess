namespace BookGuess.Domain.Entities;

public class UserAchievement
{
    public Guid UserId { get; private set; }
    public Guid AchievementId { get; private set; }
    public DateTime UnlockedAt { get; private set; }

    public User User { get; private set; } = null!;
    public Achievement Achievement { get; private set; } = null!;

    private UserAchievement() { }

    public static UserAchievement Create(Guid userId, Guid achievementId)
    {
        return new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            UnlockedAt = DateTime.UtcNow
        };
    }
}
