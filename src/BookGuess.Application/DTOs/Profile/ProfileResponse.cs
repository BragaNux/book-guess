namespace BookGuess.Application.DTOs.Profile;

public record ProfileResponse(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl,
    int Level,
    int XP,
    int CurrentStreak,
    int BestStreak,
    int TotalWins,
    IEnumerable<AchievementDto> Achievements
);

public record AchievementDto(string Code, string Name, string Description, string? Icon, DateTime UnlockedAt);
