namespace BookGuess.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public int Level { get; private set; } = 1;
    public int XP { get; private set; } = 0;
    public int CurrentStreak { get; private set; } = 0;
    public int BestStreak { get; private set; } = 0;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Match> Matches { get; private set; } = new List<Match>();
    public ICollection<UserAchievement> UserAchievements { get; private set; } = new List<UserAchievement>();

    private User() { }

    public static User Create(string name, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Level = 1,
            XP = 0,
            CurrentStreak = 0,
            BestStreak = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;
        XP += amount;
        UpdateLevel();
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void IncrementStreak()
    {
        CurrentStreak++;
        if (CurrentStreak > BestStreak)
            BestStreak = CurrentStreak;
    }

    public void ResetStreak()
    {
        CurrentStreak = 0;
    }

    private void UpdateLevel()
    {
        Level = XP switch
        {
            >= 3500 => 5,
            >= 2000 => 4,
            >= 1000 => 3,
            >= 500 => 2,
            _ => 1
        };
    }
}
