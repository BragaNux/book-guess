namespace BookGuess.Domain.Entities;

public class Achievement
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public int XPReward { get; private set; }

    public ICollection<UserAchievement> UserAchievements { get; private set; } = new List<UserAchievement>();

    private Achievement() { }

    public static Achievement Create(string code, string name, string description, int xpReward, string? icon = null)
    {
        return new Achievement
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Description = description,
            XPReward = xpReward,
            Icon = icon
        };
    }
}
