namespace BookGuess.Web.Models;

public record AuthResponse(string Token, string Name, string Email, int Level, int XP);

public record StartMatchResponse(
    Guid MatchId, string Quote, int Difficulty,
    int AttemptsRemaining, int HintsRemaining, int Score, bool IsDaily);

public record GuessResponse(
    bool IsCorrect, int AttemptsRemaining, int Score,
    string? MatchStatus, MatchResultResponse? Result);

public record MatchResultResponse(string BookTitle, string Author, int Score, int XPEarned, string? Trivia);

public record HintResponse(string Hint, int HintsRemaining, int Score);

public record RankingEntry(int Position, Guid UserId, string Name, string? AvatarUrl, int Score, int Level);

public record ProfileResponse(
    Guid Id, string Name, string Email, string? AvatarUrl,
    int Level, int XP, int CurrentStreak, int BestStreak,
    int TotalWins, IEnumerable<AchievementDto> Achievements);

public record AchievementDto(string Code, string Name, string Description, string? Icon, DateTime UnlockedAt);

public record ApiResponse<T>(bool Success, T? Data, string? Message);
