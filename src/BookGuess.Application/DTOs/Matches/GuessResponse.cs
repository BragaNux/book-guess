namespace BookGuess.Application.DTOs.Matches;

public record GuessResponse(
    bool IsCorrect,
    int AttemptsRemaining,
    int Score,
    string? MatchStatus,
    MatchResultResponse? Result
);
